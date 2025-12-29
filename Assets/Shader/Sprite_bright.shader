Shader "Triniti/Sprite_bright" {
Properties {
 _MainTex ("MainTex", 2D) = "" {}
 _TintColor ("Tint Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  BindChannels {
   Bind "vertex", Vertex
   Bind "color", Color
   Bind "texcoord", TexCoord
  }
  ZTest Always
  ZWrite Off
  Cull Off
  Fog { Mode Off }
  Blend SrcAlpha One
  SetTexture [_MainTex] { ConstantColor [_TintColor] combine texture * constant double, texture alpha * constant alpha }
 }
}
}