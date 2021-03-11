// MIT License
// Copyright (c) 2011-2016 Elisée Maurer, Sparklin Labs, Creative Patterns
// Copyright (c) 2016 Thomas Morgner, Rabbit-StewDio Ltd.
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Microsoft.Xna.Framework;

using Steropes.UI.Annotations;

namespace Steropes.UI.State
{
  public interface IGameStateManager : INotifyPropertyChanged
  {
    event EventHandler<EventArgs> SwitchingState;

    IGameState CurrentState { get; }

    bool IsSwitching { get; }

    IGameState NextState { get; }

    void Exit();

    void SwitchState(IGameState newState);
  }

  public interface INamedStateManager : IGameStateManager
  {
    INamedLookup<string, IGameState> States { get; }
  }

  /// Game Component that handles game states and their transitions
  public class GameStateManager : DrawableGameComponent, IGameStateManager
  {
    IGameState currentState;

    bool isExiting;

    IGameState nextState;

    public GameStateManager(Game game) : base(game)
    {
      game.Exiting += (sender, args) => Exit();
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public event EventHandler<EventArgs> SwitchingState;

    public IGameState CurrentState
    {
      get
      {
        return currentState;
      }
      private set
      {
        if (ReferenceEquals(value, currentState))
        {
          return;
        }
        currentState = value;
        OnPropertyChanged();
      }
    }

    public bool IsSwitching => NextState != null;

    /// <summary>
    ///   Next game state to be started, stored while the current state is fading out
    /// </summary>
    public IGameState NextState
    {
      get
      {
        return nextState;
      }
      private set
      {
        if (ReferenceEquals(value, nextState))
        {
          return;
        }
        nextState = value;
        OnPropertyChanged();
        OnPropertyChanged(nameof(IsSwitching));
      }
    }

    /// <summary>
    ///   Draws the current GameState.
    /// </summary>
    /// <param name="time"></param>
    public override void Draw(GameTime time)
    {
      if (isExiting)
      {
        CurrentState?.DrawFadeOut(time);
      }
      else if (NextState != null)
      {
        if (CurrentState != null)
        {
          CurrentState.DrawFadeOut(time);
        }
        else
        {
          NextState.DrawFadeIn(time);
        }
      }
      else
      {
        CurrentState?.Draw(time);
      }
    }

    public void Exit()
    {
      isExiting = true;
    }

    /// <summary>
    ///   Switches current GameState.
    /// </summary>
    /// <param name="newState"></param>
    public void SwitchState(IGameState newState)
    {
      NextState = newState ?? throw new ArgumentNullException(nameof(newState));

      if (CurrentState == null)
      {
        NextState.Start();
      }
    }

    /// <summary>
    ///   Updates the current GameState.
    /// </summary>
    /// <param name="time">Provides a snapshot of timing values.</param>
    public override void Update(GameTime time)
    {
      if (isExiting)
      {
        // Fade out current state
        if (CurrentState == null)
        {
          Game.Exit();
          return;
        }

        if (CurrentState.UpdateFadeOut(time))
        {
          // We're done
          CurrentState.Stop();
          CurrentState = null;
          Game.Exit();
        }
        return;
      }

      // Handle state switching
      if (NextState == null)
      {
        // Update current GameState
        CurrentState?.Update(time);
        return;
      }

      if (CurrentState != null)
      {
        // Fade out current state
        if (CurrentState.UpdateFadeOut(time))
        {
          // We're done fading out
          CurrentState.Stop();
          CurrentState = null;

          SwitchingState?.Invoke(this, EventArgs.Empty);

          NextState.Start();
        }
      }

      if (CurrentState == null)
      {
        // Fade in next state
        if (NextState.UpdateFadeIn(time))
        {
          // We're done fading in
          CurrentState = NextState;
          NextState = null;
        }
      }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }

  public class NamedGameStateManager : GameStateManager, INamedStateManager
  {
    public NamedGameStateManager(Game game) : base(game)
    {
      States = new DictionaryNamedLookup<string, IGameState, IDictionary<string, IGameState>>(new Dictionary<string, IGameState>());
    }

    public INamedLookup<string, IGameState> States { get; }
  }
}