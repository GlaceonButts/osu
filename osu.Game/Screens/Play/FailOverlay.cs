﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using System;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Transforms;
using osu.Framework.Input;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.Play.Pause;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using osu.Game.Graphics;
using osu.Framework.Allocation;

namespace osu.Game.Screens.Play
{
    public class FailOverlay : OverlayContainer
    {
        private const int transition_duration = 200;
        private const int button_height = 70;
        private const float background_alpha = 0.75f;

        protected override bool HideOnEscape => false;

        public Action OnRetry;
        public Action OnQuit;

        private string title;
        private string description;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private FillFlowContainer buttons;

        public int Retries
        {
            set
            {
                if (retryCounterContainer != null)
                {
                    // "You've retried 1,065 times in this session"
                    // "You've retried 1 time in this session"

                    retryCounterContainer.Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Text = "You've retried ",
                            Shadow = true,
                            ShadowColour = new Color4(0, 0, 0, 0.25f),
                            TextSize = 18
                        },
                        new OsuSpriteText
                        {
                            Text = $"{value:n0}",
                            Font = @"Exo2.0-Bold",
                            Shadow = true,
                            ShadowColour = new Color4(0, 0, 0, 0.25f),
                            TextSize = 18
                        },
                        new OsuSpriteText
                        {
                            Text = $" time{(value == 1 ? "" : "s")} in this session",
                            Shadow = true,
                            ShadowColour = new Color4(0, 0, 0, 0.25f),
                            TextSize = 18
                        }
                    };
                }
            }
        }

        private FillFlowContainer retryCounterContainer;

        public override bool HandleInput => State == Visibility.Visible;

        protected override void PopIn() => FadeIn(transition_duration, EasingTypes.In);
        protected override void PopOut() => FadeOut(transition_duration, EasingTypes.In);

        // Don't let mouse down events through the overlay or people can click circles while paused.
        protected override bool OnMouseDown(InputState state, MouseDownEventArgs args) => true;

        protected override bool OnMouseMove(InputState state) => true;

        protected override bool OnKeyDown(InputState state, KeyDownEventArgs args)
        {
            if (args.Key == Key.Escape)
            {
                if (State == Visibility.Hidden) return false;
                OnQuit();
                return true;
            }

            return base.OnKeyDown(state, args);
        }

        public void AddButton(string text, Color4 colour, Action action)
        {
            buttons.Add(new PauseButton
            {
                Text = text,
                ButtonColour = colour,
                Origin = Anchor.TopCentre,
                Anchor = Anchor.TopCentre,
                Height = button_height,
                Action = delegate {
                    action?.Invoke();
                    Hide();
                }
            });
        }

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Black,
                    Alpha = background_alpha,
                },
                new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 50),
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 20),
                            Children = new Drawable[]
                            {
                                new OsuSpriteText
                                {
                                    Text = Title,
                                    Font = @"Exo2.0-Medium",
                                    Spacing = new Vector2(5, 0),
                                    Origin = Anchor.TopCentre,
                                    Anchor = Anchor.TopCentre,
                                    TextSize = 30,
                                    Colour = Color4.Yellow,
                                    Shadow = true,
                                    ShadowColour = new Color4(0, 0, 0, 0.25f)
                                },
                                new OsuSpriteText
                                {
                                    Text = Description,
                                    Origin = Anchor.TopCentre,
                                    Anchor = Anchor.TopCentre,
                                    Shadow = true,
                                    ShadowColour = new Color4(0, 0, 0, 0.25f)
                                }
                            }
                        },
                        buttons = new FillFlowContainer
                        {
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Masking = true,
                            EdgeEffect = new EdgeEffect
                            {
                                Type = EdgeEffectType.Shadow,
                                Colour = Color4.Black.Opacity(0.6f),
                                Radius = 50
                            },
                        },
                        retryCounterContainer = new FillFlowContainer
                        {
                            Origin = Anchor.TopCentre,
                            Anchor = Anchor.TopCentre,
                            AutoSizeAxes = Axes.Both,
                        }
                    }
                },
                new PauseProgressBar
                {
                    Origin = Anchor.BottomCentre,
                    Anchor = Anchor.BottomCentre,
                    Width = 1f
                }
            };

            Retries = 0;

            AddButtons(colours);
        }

        protected virtual void AddButtons(OsuColour colours)
        {
            AddButton(@"Retry", colours.YellowDark, OnRetry);
            AddButton(@"Quit to Main Menu", new Color4(170, 27, 39, 255), OnQuit);
        }

        public FailOverlay()
        {
            AlwaysReceiveInput = true;
            RelativeSizeAxes = Axes.Both;            
        }
    }
}
