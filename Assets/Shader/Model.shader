Shader "Triniti/Model" {
Properties {
 _MainTex ("MainTex", 2D) = "" {}
}
SubShader { 
 Pass {
  Cull Off
  SetTexture [_MainTex] { combine texture, texture alpha }
 }
}
}