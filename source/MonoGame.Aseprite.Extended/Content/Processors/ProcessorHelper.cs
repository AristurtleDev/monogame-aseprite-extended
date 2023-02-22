/* ----------------------------------------------------------------------------
MIT License
Copyright (c) 2023 Christopher Whitley
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
---------------------------------------------------------------------------- */

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using MonoGame.Aseprite.Content.Processors;
using MonoGame.Aseprite.RawTypes;
using MonoGame.Extended;
using MonoGame.Extended.TextureAtlases;

using ExtendedTextureAtlas = MonoGame.Extended.TextureAtlases.TextureAtlas;

internal static class ProcessorHelper
{
    internal static Texture2D CreateTexture(GraphicsDevice graphicsDevice, RawTexture rawTexture)
    {
        Texture2D texture = new(graphicsDevice, rawTexture.Width, rawTexture.Height, mipmap: false, SurfaceFormat.Color);
        texture.Name = rawTexture.Name;
        texture.SetData<Color>(rawTexture.Pixels.ToArray());
        return texture;
    }

    internal static void AddRegions(RawTextureAtlas rawAtlas, ExtendedTextureAtlas extendedAtlas)
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

        // //  I don't like doing this double loop through the raw regions but this is why it's happening.
        // //  I want to include the Slices from Aseprite as TextureRegion2D elements in the extended atlas.
        // //  I could add them in during the above loop for each region, however, doing this would create additional
        // //  indexes which would desync index of the RawTextureRegion -> TextureRegion2D being 1:1 with the
        // //  frame index from Aseprite
        // foreach (RawTextureRegion rawRegion in rawAtlas.RawTextureRegions)
        // {
        //     AddSliceRegions(rawRegion.Slices, extendedAtlas);
        // }
    }

    internal static void AddSliceRegions(ReadOnlySpan<RawSlice> slices, ExtendedTextureAtlas extendedAtlas)
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