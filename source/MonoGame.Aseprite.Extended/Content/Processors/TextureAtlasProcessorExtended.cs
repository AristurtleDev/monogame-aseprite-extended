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
        Texture2D texture = ProcessorHelper.CreateTexture(graphicsDevice, rawTextureAtlas.RawTexture);
        ExtendedTextureAtlas extendedAtlas = new(rawTextureAtlas.Name, texture);
        ProcessorHelper.AddRegions(rawTextureAtlas, extendedAtlas);
        return extendedAtlas;
    }




}