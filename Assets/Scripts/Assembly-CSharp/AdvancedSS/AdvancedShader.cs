using System.Collections.Generic;

namespace AdvancedSS
{
	public class AdvancedShader
	{
		public enum ShaderEffect
		{
			ADVANCEDSS_SPECULAR = 0,
			ADVANCEDSS_SPECMAP = 1,
			ADVANCEDSS_GLOSSMAP = 2,
			ADVANCEDSS_BUMP = 3,
			ADVANCEDSS_PARALLAX = 4,
			ADVANCEDSS_POM = 5,
			ADVANCEDSS_RELIEF = 6,
			ADVANCEDSS_RIM = 7,
			ADVANCEDSS_EMISSIVE = 8,
			ADVANCEDSS_CUBEREFLECTION = 9,
			ADVANCEDSS_LIGHTINGRAMP = 10,
			ADVANCEDSS_TESSELLATIONPHONG = 11,
			ADVANCEDSS_TESSELLATIONMAPFIXED = 12,
			ADVANCEDSS_TESSELLATIONMAPDISTANCE = 13,
			ADVANCEDSS_TESSELLATIONMAPEDGELENGTH = 14
		}

		public enum ShaderQuality
		{
			HIGH = 0,
			MEDIUM = 1,
			LOW = 2
		}

		public static string[] shaderEffectNames = new string[15]
		{
			"Specular", "Spec Map", "Gloss Map", "Normal Map", "Parallax Map", "Parallax Occlusion Map", "Relaxed Cone Stepped Relief Map", "Rim Lighting", "Emissive Map", "Cubemap Reflection",
			"Lighting Ramp (Stylised lighting)", "Tessellation (Phong)", "Tessellation (Fixed)", "Tessellation (Distance)", "Tessellation (Edge Length)"
		};

		public List<ShaderEffect> shaderEffects = new List<ShaderEffect>();

		public bool mainTex = true;

		public bool color = true;

		public bool transparent;

		public bool transparentCutout;

		public string queue = "Geometry";

		public int queueOffset;

		public ShaderQuality quality;

		public bool mobile;

		public int subShader1LOD = 500;

		public int subShader2LOD = 250;

		public void SetQueueForTransparency()
		{
			if (transparent)
			{
				queue = "Transparent";
			}
			else if (transparentCutout)
			{
				queue = "AlphaTest";
			}
			else
			{
				queue = "Geometry";
			}
		}

		public string ShaderFileName()
		{
			return ShaderFileName(true);
		}

		public string ShaderFileName(bool includeExtension)
		{
			string text = string.Empty;
			if (mobile)
			{
				text += "Mobile/";
			}
			text = (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG) ? (text + "TessellationPhong/") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED) ? (text + "TessellationMapFixed/") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE) ? (text + "TessellationMapDistance/") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH) ? (text + "TessellationMapEdgeLength/") : (HasEffect(ShaderEffect.ADVANCEDSS_PARALLAX) ? (text + "Parallax/") : (HasEffect(ShaderEffect.ADVANCEDSS_RELIEF) ? (text + "RCSM/") : (HasEffect(ShaderEffect.ADVANCEDSS_POM) ? (text + "ParallaxOcclusion/") : ((!HasEffect(ShaderEffect.ADVANCEDSS_BUMP)) ? (text + "Standard/") : (text + "Bump/")))))))));
			if (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG) || HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED) || HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE) || HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH))
			{
				if (HasEffect(ShaderEffect.ADVANCEDSS_PARALLAX))
				{
					text += "Parallax/";
				}
				else if (HasEffect(ShaderEffect.ADVANCEDSS_RELIEF))
				{
					text += "RCSM/";
				}
				else if (HasEffect(ShaderEffect.ADVANCEDSS_POM))
				{
					text += "ParallaxOcclusion/";
				}
				else if (HasEffect(ShaderEffect.ADVANCEDSS_BUMP))
				{
					text += "Bump/";
				}
			}
			if (mobile)
			{
				text += "Mobile-";
			}
			if (transparent)
			{
				text += "Alpha-";
			}
			else if (transparentCutout)
			{
				text += "AlphaTest-";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_CUBEREFLECTION))
			{
				text += "Reflect-";
			}
			text = (HasEffect(ShaderEffect.ADVANCEDSS_PARALLAX) ? (text + "Parallax") : (HasEffect(ShaderEffect.ADVANCEDSS_RELIEF) ? (text + "RCSM") : (HasEffect(ShaderEffect.ADVANCEDSS_POM) ? (text + "ParallaxOcclusion") : (HasEffect(ShaderEffect.ADVANCEDSS_BUMP) ? (text + "Bumped") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG) ? (text + "TessellationPhong") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED) ? (text + "TessellationMapFixed") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE) ? (text + "TessellationMapDistance") : ((!HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH)) ? (text + "Diffuse") : (text + "TessellationMapEdgeLength")))))))));
			if (HasEffect(ShaderEffect.ADVANCEDSS_SPECMAP))
			{
				text += "SpecMap";
			}
			else if (HasEffect(ShaderEffect.ADVANCEDSS_GLOSSMAP))
			{
				text += "GlossMap";
			}
			else if (HasEffect(default(ShaderEffect)))
			{
				text += "Specular";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_EMISSIVE))
			{
				text += "Emissive";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_RIM))
			{
				text += "Rim";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_LIGHTINGRAMP))
			{
				text += "-Ramp";
			}
			text = text.TrimEnd('-');
			if (includeExtension)
			{
				text += ".shader";
			}
			return text;
		}

		public string ShaderName()
		{
			string text = "Advanced SS/";
			if (mobile)
			{
				text += "Mobile/";
			}
			text = (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG) ? (text + "Tessellation (Phong)/") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED) ? (text + "Tessellation Map (Fixed)/") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE) ? (text + "Tessellation Map (Distance)/") : (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH) ? (text + "Tessellation Map (Edge Length)/") : (HasEffect(ShaderEffect.ADVANCEDSS_PARALLAX) ? (text + "Parallax/") : (HasEffect(ShaderEffect.ADVANCEDSS_RELIEF) ? (text + "Relaxed Cone Stepping/") : (HasEffect(ShaderEffect.ADVANCEDSS_POM) ? (text + "Parallax Occlusion (D3D)/") : ((!HasEffect(ShaderEffect.ADVANCEDSS_BUMP)) ? (text + "Standard/") : (text + "Bump/")))))))));
			if (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG) || HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED) || HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE) || HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH))
			{
				if (HasEffect(ShaderEffect.ADVANCEDSS_PARALLAX))
				{
					text += "Parallax/";
				}
				else if (HasEffect(ShaderEffect.ADVANCEDSS_RELIEF))
				{
					text += "Relaxed Cone Stepping/";
				}
				else if (HasEffect(ShaderEffect.ADVANCEDSS_POM))
				{
					text += "Parallax Occlusion (D3D)/";
				}
				else if (HasEffect(ShaderEffect.ADVANCEDSS_BUMP))
				{
					text += "Bump/";
				}
			}
			if (transparent)
			{
				text += "Transparent/";
			}
			else if (transparentCutout)
			{
				text += "Transparent/Cutout/";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_CUBEREFLECTION))
			{
				text += "Reflective/";
			}
			text = (HasEffect(ShaderEffect.ADVANCEDSS_SPECMAP) ? (text + "SpecMap") : (HasEffect(ShaderEffect.ADVANCEDSS_GLOSSMAP) ? (text + "GlossMap") : ((!HasEffect(default(ShaderEffect))) ? (text + "Diffuse") : (text + "Specular"))));
			if (HasEffect(ShaderEffect.ADVANCEDSS_EMISSIVE))
			{
				text += " Emissive";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_RIM))
			{
				text += " Rim";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_LIGHTINGRAMP))
			{
				text += " Ramp";
			}
			return text;
		}

		public void CreateShaderFile(string directory)
		{
		}

		public bool HasEffect(params ShaderEffect[] effects)
		{
			for (int i = 0; i < shaderEffects.Count; i++)
			{
				for (int j = 0; j < effects.Length; j++)
				{
					if (shaderEffects[i] == effects[j])
					{
						return true;
					}
				}
			}
			return false;
		}

		public string GenerateShaderCode()
		{
			string empty = string.Empty;
			bool flag = HasEffect(ShaderEffect.ADVANCEDSS_SPECULAR, ShaderEffect.ADVANCEDSS_SPECMAP, ShaderEffect.ADVANCEDSS_GLOSSMAP);
			bool flag2 = HasEffect(ShaderEffect.ADVANCEDSS_PARALLAX, ShaderEffect.ADVANCEDSS_POM, ShaderEffect.ADVANCEDSS_RELIEF);
			bool flag3 = HasEffect(ShaderEffect.ADVANCEDSS_BUMP, ShaderEffect.ADVANCEDSS_PARALLAX, ShaderEffect.ADVANCEDSS_POM, ShaderEffect.ADVANCEDSS_RELIEF);
			bool flag4 = HasEffect(ShaderEffect.ADVANCEDSS_SPECMAP, ShaderEffect.ADVANCEDSS_GLOSSMAP, ShaderEffect.ADVANCEDSS_PARALLAX, ShaderEffect.ADVANCEDSS_POM);
			bool flag5 = HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG, ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED, ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE, ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH);
			bool flag6 = HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED, ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE, ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH);
			string text = string.Empty;
			if (flag5 && flag3)
			{
				text += "../";
			}
			if (mobile)
			{
				text += "../";
			}
			empty = empty + "Shader \"" + ShaderName() + "\"\r\n{\r\n    Properties\r\n    {\r\n";
			if (color && !mobile)
			{
				empty += "        _Color (\"Main Color\", Color) = (1,1,1,1)\r\n";
			}
			if (flag)
			{
				if (!mobile)
				{
					empty += "        _SpecColor (\"Specular Color\", Color) = (0.5, 0.5, 0.5, 1)\r\n";
				}
				empty += "        _Shininess (\"Shininess\", Range (0.01, 1)) = 0.078125\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_CUBEREFLECTION))
			{
				empty += "        _ReflectColor (\"Reflection Color\", Color) = (1,1,1,0.5)\r\n";
			}
			if (flag2)
			{
				empty += "        _Parallax (\"Height\", Range (0.005, 0.08)) = 0.08\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_POM))
			{
				empty += "        _ParallaxSamples (\"Parallax Samples\", Range (10, 50)) = 40\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED) || HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE))
			{
				empty += "        _Tess (\"Tessellation\", Range(1,32)) = 4\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE))
			{
				empty += "        _TessDistanceMin (\"Tessellation Min Distance\", Float) = 10\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE))
			{
				empty += "        _TessDistanceMax (\"Tessellation Max Distance\", Float) = 25\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG) || HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH))
			{
				empty += "        _EdgeLength (\"Edge length\", Range(2,50)) = 5\r\n";
			}
			if (flag6)
			{
				empty += "        _Displacement (\"Displacement\", Range(0, 1.0)) = 0.3\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG))
			{
				empty += "        _Phong (\"Phong Strength\", Range(0,1)) = 0.54\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_RIM))
			{
				empty += "        _RimColor (\"Rim Color\", Color) = (0.75,0.75,0.75,0.0)\r\n        _RimPower (\"Rim Power\", Range(0.5,8.0)) = 3.0\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_EMISSIVE))
			{
				empty += "        _EmissiveStrength (\"Emissive Strength\", Float) = 1.0\r\n";
			}
			if (mainTex)
			{
				empty += "        _MainTex (\"Texture\", 2D) = \"white\" {}\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_CUBEREFLECTION))
			{
				empty += "        _Cube (\"Reflection Cubemap\", Cube) = \"_Skybox\" { TexGen CubeReflect }\r\n";
			}
			if (flag3)
			{
				empty += "        _BumpMap (\"Bumpmap\", 2D) = \"bump\" {}\r\n";
			}
			if (flag4 || flag6)
			{
				string text2 = string.Empty;
				if (HasEffect(ShaderEffect.ADVANCEDSS_PARALLAX, ShaderEffect.ADVANCEDSS_PARALLAX))
				{
					text2 = ", Heightmap (A)";
				}
				empty = (HasEffect(ShaderEffect.ADVANCEDSS_SPECMAP) ? (empty + "        _SpecMap (\"SpecMap (RGB)" + text2 + "\", 2D) = \"white\" {}\r\n") : ((!HasEffect(ShaderEffect.ADVANCEDSS_GLOSSMAP)) ? (empty + "        _SpecMap (\"Heightmap (A)\", 2D) = \"white\" {}\r\n") : (empty + "        _SpecMap (\"Gloss (R), Shininess (G)" + text2 + "\", 2D) = \"white\" {}\r\n")));
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_RELIEF))
			{
				empty += "        _RelaxedConeMap (\"RelaxedConeMap\", 2D) = \"white\" {}\r\n        _ClipTiling (\"Relief Clip Tiling U,V\", Vector) = (1, 1, 0, 0)\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_EMISSIVE))
			{
				empty += "        _EmissiveMap (\"EmissiveMap (RGB)\", 2D) = \"black\" {}\r\n";
			}
			if (HasEffect(ShaderEffect.ADVANCEDSS_LIGHTINGRAMP))
			{
				empty += "        _LightingRamp (\"Lighting Ramp (RGB)\", 2D) = \"white\" {}\r\n";
			}
			if (transparentCutout)
			{
				empty += "        _Cutoff (\"Alpha cutoff\", Range(0,1)) = 0.5\r\n";
			}
			empty += "    }\r\n\r\n    SubShader\r\n    {\r\n";
			string text3 = queue;
			if (queueOffset > 0)
			{
				text3 = text3 + "+" + queueOffset;
			}
			else if (queueOffset < 0)
			{
				text3 = text3 + "-" + queueOffset;
			}
			empty = (transparent ? (empty + "        Tags {\"Queue\"=\"" + text3 + "\" \"IgnoreProjector\"=\"True\" \"RenderType\"=\"Transparent\"}\r\n") : ((!transparentCutout) ? (empty + "        Tags { \"Queue\"=\"" + text3 + "\" \"RenderType\"=\"Opaque\"}\r\n") : (empty + "        Tags {\"Queue\"=\"" + text3 + "\" \"IgnoreProjector\"=\"True\" \"RenderType\"=\"TransparentCutout\"}\r\n")));
			empty = empty + "        LOD " + subShader1LOD + "\r\n\r\n        CGPROGRAM\r\n\r\n";
			if (mobile)
			{
				empty += "        #define ADVANCEDSS_MOBILE\r\n";
			}
			for (int i = 0; i < shaderEffects.Count; i++)
			{
				empty = empty + "        #define " + shaderEffects[i].ToString() + "\r\n";
			}
			string text4 = "BlinnPhong";
			if (!flag)
			{
				text4 = ((!HasEffect(ShaderEffect.ADVANCEDSS_LIGHTINGRAMP)) ? "Lambert" : "LambertRamp");
			}
			else
			{
				if (mobile)
				{
					text4 = "MobileBlinnPhong";
				}
				if (HasEffect(ShaderEffect.ADVANCEDSS_LIGHTINGRAMP))
				{
					text4 += "Ramp";
				}
			}
			empty += "\r\n";
			if (HasEffect(ShaderEffect.ADVANCEDSS_CUBEREFLECTION) && HasEffect(ShaderEffect.ADVANCEDSS_RELIEF))
			{
				empty += "        #pragma exclude_renderers d3d11_9x\r\n";
			}
			if (!mobile)
			{
				empty = ((!flag5) ? (empty + "        #if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)\r\n        #pragma target 5.0\r\n        #else\r\n        #pragma target 3.0\r\n        #endif\r\n") : (empty + "        #pragma target 5.0\r\n"));
			}
			string text5 = empty;
			empty = text5 + string.Empty + ((!HasEffect(ShaderEffect.ADVANCEDSS_POM)) ? string.Empty : "        #pragma only_renderers d3d9 d3d11 d3d11_9x\r\n") + string.Empty + ((!HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE, ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG, ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH)) ? string.Empty : "        #include \"Tessellation.cginc\"\r\n") + "        #include \"" + text + "../AdvancedSS.cginc\"\r\n        #pragma surface advancedSurfaceShader " + text4 + ((!mobile || flag) ? string.Empty : " noforwardadd") + ((!mobile || !flag) ? string.Empty : " exclude_path:prepass nolightmap noforwardadd halfasview") + ((!HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPFIXED)) ? string.Empty : " addshadow fullforwardshadows vertex:Disp tessellate:TessFixed") + ((!HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPDISTANCE)) ? string.Empty : " addshadow fullforwardshadows vertex:Disp tessellate:TessDistance") + ((!HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONMAPEDGELENGTH)) ? string.Empty : " addshadow fullforwardshadows vertex:Disp tessellate:TessEdge") + ((!HasEffect(ShaderEffect.ADVANCEDSS_TESSELLATIONPHONG)) ? string.Empty : " vertex:DispNone tessellate:TessEdge tessphong:_Phong") + ((!transparentCutout) ? string.Empty : " alphatest:_Cutoff") + ((!flag5) ? string.Empty : " nolightmap") + "\r\n\r\n        ENDCG\r\n    }\r\n\r\n";
			empty = (mobile ? (empty + "    Fallback \"Mobile/VertexLit\"\r\n") : ((!flag) ? (empty + "    Fallback \"Diffuse\"\r\n") : (empty + "    Fallback \"Specular\"\r\n")));
			return empty + "}";
		}
	}
}
