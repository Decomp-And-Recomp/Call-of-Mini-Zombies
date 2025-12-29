Shader "iPhone/LightMap_Effect" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _texBase ("MainTex", 2D) = "" {}
 _texLightmap ("LightMap", 2D) = "" {}
 _texEffect ("EffectMap", 2D) = "" {}
}
SubShader { 
 Pass {
  BindChannels {
   Bind "vertex", Vertex
   Bind "texcoord", TexCoord0
   Bind "texcoord1", TexCoord1
   Bind "texcoord", TexCoord2
  }
  SetTexture [_texBase] { ConstantColor [_Color] combine texture * constant }
  SetTexture [_texLightmap] { combine texture * previous }
  SetTexture [_texEffect] { combine texture * previous + previous }
 }
}
}