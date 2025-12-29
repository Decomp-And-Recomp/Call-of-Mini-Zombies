Shader "iPhone/Additive" {
Properties {
 _MainTex ("Texture", 2D) = "white" {}
}
SubShader { 
 Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent+1" "IGNOREPROJECTOR"="True" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha One
  SetTexture [_MainTex] { combine texture }
 }
}
}