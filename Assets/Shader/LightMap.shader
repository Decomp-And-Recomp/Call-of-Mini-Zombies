Shader "iPhone/LightMap" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _texBase ("MainTex", 2D) = "" {}
 _texLightmap ("LightMap", 2D) = "White" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent-1" }
 Pass {
  Tags { "QUEUE"="Transparent-1" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "texcoord", TexCoord0
   Bind "texcoord1", TexCoord1
  }
  SetTexture [_texBase] { ConstantColor [_Color] combine texture * constant }
  SetTexture [_texLightmap] { combine texture * previous }
 }
}
}