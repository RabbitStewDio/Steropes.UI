using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Steropes.UI.Platform
{
  /// <summary>
  ///   Merges textures into one or more texture atlases. This class arranges
  ///   textures into bands and splits those bands into lines. It works best
  ///   with textures of uniform sizes.
  /// </summary>
  public class TextureAtlasBuilder
  {
    class TreeNode
    {
      IUITexture Texture { get; set; }
      Rectangle CellBounds { get; }
      TreeNode Left { get; set; }
      TreeNode Right { get; set; }
      bool Leaf => Left == null && Right == null;


      public TreeNode(Rectangle bounds)
      {
        CellBounds = bounds;
      }

      public TreeNode Insert(IUITexture tex, int padding)
      {
        if (!Leaf)
        {
          var maybeLeft = Left.Insert(tex, padding);
          if (maybeLeft != null)
          {
            return maybeLeft;
          }
          return Right.Insert(tex, padding);
        }

        if (Texture != null)
        {
          // There is already something here 
          return null;
        }
        var texBounds = tex.Bounds;
        if (texBounds.Width > CellBounds.Width ||
            texBounds.Height > CellBounds.Height)
        {
          // does not fit into the available space
          return null;
        }
        if (texBounds.Width == CellBounds.Width &&
            texBounds.Height == CellBounds.Height)
        {
          Texture = tex;
          return this;
        }
        if ((CellBounds.Width - texBounds.Width) > (CellBounds.Height - texBounds.Height))
        {
          // vertical split 
          Left = new TreeNode(new Rectangle(CellBounds.X, CellBounds.Y, texBounds.Width, CellBounds.Height));
          Right = new TreeNode(new Rectangle(CellBounds.X + padding + texBounds.Width, CellBounds.Y,
                                             CellBounds.Width - texBounds.Width - padding, CellBounds.Height));
        }
        else
        {
          Left = new TreeNode(new Rectangle(CellBounds.X, CellBounds.Y, CellBounds.Width, texBounds.Height));
          Right = new TreeNode(new Rectangle(CellBounds.X, CellBounds.Y + padding + texBounds.Height, CellBounds.Width,
                                             CellBounds.Height - texBounds.Height - padding));
        }
        return Left.Insert(tex, padding);
      }

      public IUITexture Harvest(RenderTarget2D targetTexture)
      {
        if (Texture != null)
        {
          // copy into targetTexture and return new ITexturedTile instance..
          var g = targetTexture.GraphicsDevice;
          g.SetRenderTarget(targetTexture);

          SpriteBatch b = new SpriteBatch(g);
          b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp);
          b.Draw(Texture.Texture, CellBounds, Texture.Bounds, Color.White);
          b.End();

          g.SetRenderTarget(null);
          return Texture.Rebase(targetTexture, CellBounds, Texture.Name);
        }

        var l = Left.Harvest(targetTexture);
        return l ?? Right.Harvest(targetTexture);
      }
    }

    public const int MaxTextureSize = 2048;

    readonly TreeNode root;
    readonly RenderTarget2D texture;

    public TextureAtlasBuilder(RenderTarget2D texture, Rectangle maxSize)
    {
      root = new TreeNode(maxSize);
      this.texture = texture;
    }

    public TextureAtlasBuilder(RenderTarget2D device, int maxSize) : this(device, new Rectangle(0, 0, maxSize, maxSize))
    {
    }


    public bool Insert(IUITexture tile, out IUITexture result)
    {
      if (!IsValid(tile))
      {
        result = tile;
        return true;
      }

      var res = root.Insert(tile, 2);
      if (res != null)
      {
        result = res.Harvest(texture);
        return result != null;
      }

      result = tile;
      return false;
    }


    static bool IsValid(IUITexture tex)
    {
      return tex?.Texture != null;
    }

    public void Save(string filename)
    {
      using (Stream stream = File.Create(filename))
      {
        texture.SaveAsPng(stream, texture.Width, texture.Height);
      }
    }
  }
}