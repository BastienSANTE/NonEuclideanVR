Shader "Custom/Portal"
{
    Properties
    {
        _InactiveColour ("Inactive Colour", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // Déclare le shader comme opaque (même si on utilise une texture dynamique).
        Tags { "RenderType"="Opaque" }
        // Niveau de détail standard
        LOD 100
        // Désactive le culling des faces arrière. Cela signifie que les deux côtés du portail seront visibles, 
        // ce qui est utile pour un effet de portail bidirectionnel.
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            //  Définit la fonction vertex shader (vert())
            #pragma fragment frag
            // Définit la fonction fragment shader (frag())
            #include "UnityCG.cginc"
            // Importation des fonctions utiles de Unity (ex: UnityObjectToClipPos et ComputeScreenPos).

            struct appdata
            {
                float4 vertex : POSITION;
                // Cette structure ne contient que la position du sommet, utilisée pour calculer sa transformation
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                // Position finale en espace écran (utilisée pour le rendu).
                float4 screenPos : TEXCOORD0;
                // Stocke la position en coordonnées écran pour ensuite être utilisée dans le fragment
                // shader afin de générer des UVs dynamiques.
            };

            sampler2D _MainTex;
            float4 _InactiveColour;
            int displayMask; // set to 1 to display texture, otherwise will draw test colour
            

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Cette fonction transforme la position du modèle vers l’espace écran.
                o.screenPos = ComputeScreenPos(o.vertex);
                // ComputeScreenPos() convertit SV_POSITION en coordonnées écran homogènes.
                // Ces coordonnées seront utilisées dans le fragment shader pour générer des UVs dynamiques.
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.screenPos.xy / i.screenPos.w;
                fixed4 portalCol = tex2D(_MainTex, uv);
                return portalCol * displayMask + _InactiveColour * (1-displayMask);
            }
            ENDCG
        }
    }
    Fallback "Standard" // for shadows
}