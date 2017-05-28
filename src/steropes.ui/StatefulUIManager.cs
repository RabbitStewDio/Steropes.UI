﻿// MIT License
//
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Microsoft.Xna.Framework.Content;
using Steropes.UI.Components.Window;
using Steropes.UI.Input;
using Steropes.UI.Platform;
using Steropes.UI.State;

namespace Steropes.UI
{
  public class StatefulUIManager : UIManager
  {
    readonly IGameStateManager stateManager;

    public StatefulUIManager(
      IInputManager inputManager,
      IBatchedDrawingService drawingService,
      IGameWindowService windowService,
      ContentManager contentManager,
      IGameStateManager stateManager) : base(inputManager, drawingService, windowService, contentManager)
    {
      this.stateManager = stateManager;
    }

    public override bool IsActive => base.IsActive && !(this.stateManager?.IsSwitching ?? false);
  }
}