Shader "iPhone/SolidAndAlphaTextureEx" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("MainTex", 2D) = "" {}
 _tex2 ("Texture2", 2D) = "" {}
}
SubShader { 
 Pass {
  Tags { "RenderType"="Geometry" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "texcoord", TexCoord0
   Bind "texcoord1", TexCoord1
  }
  Color [_TintColor]
  SetTexture [_tex2] { combine texture * primary }
  SetTexture [_MainTex] { combine previous + texture }
 }
}
}