﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LibreLancer.Shaders
{
    using System;
    
    public class Projectile
    {
        private static byte[] vertex_bytes = new byte[173] {
27, 95, 1, 0, 196, 130, 117, 245, 45, 105, 206, 104, 114, 126, 55, 136, 235, 45, 20, 123, 136, 221, 63, 190, 125, 127, 161, 75, 130, 145,
154, 102, 146, 214, 36, 33, 50, 119, 21, 15, 242, 198, 109, 187, 33, 82, 138, 114, 52, 174, 24, 84, 157, 94, 53, 26, 119, 219, 172, 44,
137, 56, 208, 7, 28, 218, 158, 240, 90, 206, 61, 188, 150, 183, 199, 242, 51, 70, 88, 208, 8, 62, 25, 18, 51, 184, 62, 182, 199, 243,
182, 116, 66, 165, 243, 243, 145, 220, 160, 244, 7, 65, 61, 248, 90, 8, 50, 58, 141, 245, 227, 244, 225, 195, 245, 97, 222, 145, 114, 161,
173, 0, 233, 63, 101, 133, 254, 4, 0, 235, 195, 248, 42, 99, 22, 129, 50, 84, 53, 51, 153, 196, 208, 225, 40, 129, 128, 100, 89, 163,
100, 198, 79, 75, 248, 73, 8, 117, 97, 147, 55, 136, 152, 157, 0, 236, 95, 17, 249, 4, 151, 191, 16
};
        private static byte[] fragment_bytes = new byte[138] {
27, 193, 0, 0, 140, 212, 70, 117, 72, 126, 139, 145, 133, 117, 229, 91, 73, 139, 39, 53, 8, 95, 33, 218, 236, 192, 2, 133, 43, 106,
81, 216, 90, 214, 214, 146, 23, 125, 215, 135, 215, 27, 230, 16, 110, 107, 37, 137, 88, 27, 255, 24, 182, 78, 84, 4, 101, 63, 238, 98,
199, 15, 90, 56, 108, 79, 75, 148, 224, 53, 146, 41, 188, 221, 100, 31, 31, 203, 143, 168, 193, 246, 132, 175, 229, 92, 198, 209, 242, 6,
214, 120, 24, 25, 169, 227, 157, 15, 231, 91, 228, 249, 249, 208, 14, 111, 211, 181, 227, 156, 163, 5, 37, 179, 67, 193, 31, 8, 134, 163,
81, 101, 31, 207, 219, 146, 172, 118, 177, 82, 20, 210, 0, 179, 177, 94, 1, 8
};
        static ShaderVariables[] variants;
        private static bool iscompiled = false;
        private static int GetIndex(ShaderFeatures features)
        {
            ShaderFeatures masked = (features & ((ShaderFeatures)(0)));
            return 0;
        }
        public static ShaderVariables Get(ShaderFeatures features)
        {
            Compile();
            return variants[GetIndex(features)];
        }
        public static ShaderVariables Get()
        {
            Compile();
            return variants[0];
        }
        public static void Compile()
        {
            if (iscompiled)
            {
                return;
            }
            iscompiled = true;
            ShaderVariables.Log("Compiling Projectile");
            string vertsrc;
            string fragsrc;
            vertsrc = ShCompHelper.FromArray(vertex_bytes);
            fragsrc = ShCompHelper.FromArray(fragment_bytes);
            variants = new ShaderVariables[1];
            variants[0] = ShaderVariables.Compile(vertsrc, fragsrc, "");
        }
    }
}
