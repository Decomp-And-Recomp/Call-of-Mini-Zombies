Shader "iPhone/SolidAndBrightTexture" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _texBase ("MainTex", 2D) = "" {}
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
  SetTexture [_texBase] { combine primary * texture }
  SetTexture [_tex2] { combine previous + texture }
 }
}
}