using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Aseprite;
using MonoGame.Aseprite.RawTypes;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

using ExtendedSpriteSheet = MonoGame.Extended.Sprites.SpriteSheet;
using ExtendedTextureAtlas = MonoGame.Extended.TextureAtlases.TextureAtlas;

namespace MonoGame.Aseprite.Content.Processors;

public static class SpriteSheetProcessorExtended
{
    public static ExtendedSpriteSheet Process(GraphicsDevice graphicsDevice, AsepriteFile aseFile, bool onlyVisibleLayers = true, bool includeBackgroundLayer = false, bool includeTilemapLayers = true, bool mergeDuplicates = true, int borderPadding = 0, int spacing = 0, int innerPadding = 0)
    {
        RawSpriteSheet rawSpriteSheet = SpriteSheetProcessor.ProcessRaw(aseFile, onlyVisibleLayers, includeBackgroundLayer, includeTilemapLayers, mergeDuplicates, borderPadding, spacing, innerPadding);
        Texture2D texture2D = ProcessorHelper.CreateTexture(graphicsDevice, rawSpriteSheet.RawTextureAtlas.RawTexture);

        ExtendedTextureAtlas extendedTextureAtlas = new(rawSpriteSheet.RawTextureAtlas.Name, texture2D);
        ProcessorHelper.AddRegions(rawSpriteSheet.RawTextureAtlas, extendedTextureAtlas);

        ExtendedSpriteSheet extendedSpriteSheet = new();
        extendedSpriteSheet.TextureAtlas = extendedTextureAtlas;
        TagsToAnimationCycles(rawSpriteSheet, extendedSpriteSheet);

        return extendedSpriteSheet;
    }

    private static void TagsToAnimationCycles(RawSpriteSheet rawSpriteSheet, ExtendedSpriteSheet extendedSpriteSheet)
    {
        foreach (RawAnimationTag rawTag in rawSpriteSheet.RawAnimationTags)
        {
            SpriteSheetAnimationCycle cycle = new();
            cycle.FrameDuration = rawTag.RawAnimationFrames[0].DurationInMilliseconds / 1000.0f;
            cycle.IsLooping = rawTag.IsLooping;
            cycle.IsReversed = rawTag.IsReversed;
            cycle.IsPingPong = rawTag.IsPingPong;

            foreach (RawAnimationFrame rawFrame in rawTag.RawAnimationFrames)
            {
                SpriteSheetAnimationFrame extendedFrame = new(rawFrame.FrameIndex, rawFrame.DurationInMilliseconds / 1000.0f);
                cycle.Frames.Add(extendedFrame);
            }

            extendedSpriteSheet.Cycles.Add(rawTag.Name, cycle);
        }
    }
}