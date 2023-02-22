using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Aseprite;
using MonoGame.Aseprite.RawTypes;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;

using ExtendedTextureAtlas = MonoGame.Extended.TextureAtlases.TextureAtlas;

namespace MonoGame.Aseprite.Content.Processors;

public static class TextureAtlasProcessorExtended
{
    public static ExtendedTextureAtlas Process(GraphicsDevice graphicsDevice, AsepriteFile aseFile, bool onlyVisibleLayers = true, bool includeBackgroundLayer = false, bool includeTilemapLayers = true, bool mergeDuplicates = true, int borderPadding = 0, int spacing = 0, int innerPadding = 0)
    {
        RawTextureAtlas rawTextureAtlas = TextureAtlasProcessor.ProcessRaw(aseFile, onlyVisibleLayers, includeBackgroundLayer, includeTilemapLayers, mergeDuplicates, borderPadding, spacing, innerPadding);
        Texture2D texture = CreateTexture(graphicsDevice, rawTextureAtlas.RawTexture);
        ExtendedTextureAtlas extendedAtlas = new(rawTextureAtlas.Name, texture);
        AddRegions(rawTextureAtlas, extendedAtlas);
        return extendedAtlas;
    }

    private static Texture2D CreateTexture(GraphicsDevice graphicsDevice, RawTexture rawTexture)
    {
        Texture2D texture = new(graphicsDevice, rawTexture.Width, rawTexture.Height, mipmap: false, SurfaceFormat.Color);
        texture.SetData<Color>(rawTexture.Pixels.ToArray());
        texture.Name = rawTexture.Name;
        return texture;
    }

    private static void AddRegions(RawTextureAtlas rawAtlas, ExtendedTextureAtlas extendedAtlas)
    {
        //  Adding all slice regions to a list here, then adding them into the extended texture atlas
        //  after all raw texture regions are added.
        //  This is so we can keep the raw texture regions indexes 1:1 with the frame indexes from the
        //  Aseprite file and have the slice regions be supplementary regions that occur after those indexes.
        List<TextureRegion2D> sliceRegions = new();

        foreach (RawTextureRegion rawRegion in rawAtlas.RawTextureRegions)
        {
            rawRegion.Bounds.Deconstruct(out int x, out int y, out int w, out int h);
            extendedAtlas.CreateRegion(rawRegion.Name, x, y, w, h);
        }

        //  I don't like doing this double loop through the raw regions but this is why it's happening.
        //  I want to include the Slices from Aseprite as TextureRegion2D elements in the extended atlas.
        //  I could add them in during the above loop for each region, however, doing this would create additional
        //  indexes which would desync index of the RawTextureRegion -> TextureRegion2D being 1:1 with the
        //  frame index from Aseprite
        foreach (RawTextureRegion rawRegion in rawAtlas.RawTextureRegions)
        {
            AddSliceRegions(rawRegion.Slices, extendedAtlas);
        }
    }

    private static void AddSliceRegions(ReadOnlySpan<RawSlice> slices, ExtendedTextureAtlas extendedAtlas)
    {
        foreach (RawSlice slice in slices)
        {
            slice.Bounds.Deconstruct(out int x, out int y, out int w, out int h);
            if (slice is RawNinePatchSlice ninePatchSlice)
            {
                ninePatchSlice.CenterBounds.Deconstruct(out int cx, out int cy, out int cw, out int ch);
                Thickness thickness = new(cx, cy, cx + cw, cy + ch);

                extendedAtlas.CreateNinePatchRegion(ninePatchSlice.Name, x, y, w, h, thickness);
            }
            else
            {
                extendedAtlas.CreateRegion(slice.Name, x, y, w, h);
            }
        }
    }
}