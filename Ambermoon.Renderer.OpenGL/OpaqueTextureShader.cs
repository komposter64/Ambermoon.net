﻿/*
 * OpaqueTextureShader.cs - Shader for opaque textured objects
 *
 * Copyright (C) 2020-2021  Robert Schneckenhaus <robert.schneckenhaus@web.de>
 *
 * This file is part of Ambermoon.net.
 *
 * Ambermoon.net is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Ambermoon.net is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Ambermoon.net. If not, see <http://www.gnu.org/licenses/>.
 */

namespace Ambermoon.Renderer
{
    internal class OpaqueTextureShader : TextureShader
    {
        static string[] OpaqueTextureFragmentShader(State state) => new string[]
        {
            GetFragmentShaderHeader(state),
            $"uniform float {DefaultUsePaletteName};",
            $"uniform float {DefaultPaletteCountName};",
            $"uniform sampler2D {DefaultSamplerName};",
            $"uniform sampler2D {DefaultPaletteName};",
            $"in vec2 varTexCoord;",
            $"flat in float palIndex;",
            $"",
            $"void main()",
            $"{{",
            $"    vec4 pixelColor;",
            $"    if ({DefaultUsePaletteName} > 0.5f)",
            $"    {{",
            $"        float colorIndex = texture({DefaultSamplerName}, varTexCoord).r * 255.0f;",
            $"        pixelColor = texture({DefaultPaletteName}, vec2((colorIndex + 0.5f) / 32.0f, (palIndex + 0.5f) / {DefaultPaletteCountName}));",
            $"    }}",
            $"    else",
            $"    {{",
            $"        pixelColor = texture({DefaultSamplerName}, varTexCoord);",
            $"    }}",
            $"    ",
            $"    {DefaultFragmentOutColorName} = pixelColor;",
            $"}}"
        };

        protected static string[] OpaqueTextureVertexShader(State state) => new string[]
        {
            GetVertexShaderHeader(state),
            $"in vec2 {DefaultPositionName};",
            $"in ivec2 {DefaultTexCoordName};",
            $"in uint {DefaultLayerName};",
            $"in uint {DefaultPaletteIndexName};",
            $"in uint {DefaultMaskColorIndexName};",
            $"uniform uvec2 {DefaultAtlasSizeName};",
            $"uniform float {DefaultZName};",
            $"uniform mat4 {DefaultProjectionMatrixName};",
            $"uniform mat4 {DefaultModelViewMatrixName};",
            $"out vec2 varTexCoord;",
            $"flat out float palIndex;",
            $"",
            $"void main()",
            $"{{",
            $"    vec2 atlasFactor = vec2(1.0f / float({DefaultAtlasSizeName}.x), 1.0f / float({DefaultAtlasSizeName}.y));",
            $"    vec2 pos = vec2({DefaultPositionName}.x + 0.49f, {DefaultPositionName}.y + 0.49f);",
            $"    varTexCoord = atlasFactor * vec2({DefaultTexCoordName}.x, {DefaultTexCoordName}.y);",
            $"    palIndex = float({DefaultPaletteIndexName});",
            $"    gl_Position = {DefaultProjectionMatrixName} * {DefaultModelViewMatrixName} * vec4(pos, 1.0f - {DefaultZName} - float({DefaultLayerName}) * 0.00001f, 1.0f);",
            $"}}"
        };

        OpaqueTextureShader(State state)
            : base(state, OpaqueTextureFragmentShader(state), OpaqueTextureVertexShader(state))
        {

        }

        public new static OpaqueTextureShader Create(State state) => new OpaqueTextureShader(state);
    }
}
