// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Screens.Edit.Components;
using osuTK;

namespace osu.Game.Skinning.Editor
{
    internal class SkinSettingsToolbox : EditorSidebarSection
    {
        protected override Container<Drawable> Content { get; }

        public SkinSettingsToolbox()
            : base("Settings")
        {
            base.Content.Add(Content = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
            });
        }
    }
}
