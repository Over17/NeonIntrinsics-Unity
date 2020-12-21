using Unity.Burst;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

public class DotProd_Neon : MonoBehaviour
{
    void Start()
    {
        
    }

    [BurstCompile]
    public static class CalculateDotProd
    {
        public static unsafe int dotProductScalar(short* inputArray1, short* inputArray2, short len)
        {
            int result = 0;

            for (short i = 0; i < len; i++)
            {
                result += inputArray1[i] * inputArray2[i];
            }

            return result;
        }

        [BurstCompile]
        public static unsafe int dotProductBurst(short* inputArray1, short* inputArray2, short len)
        {
            int result = 0;

            for (short i = 0; i < len; i++)
            {
                result += inputArray1[i] * inputArray2[i];
            }

            return result;
        }

        [BurstCompile]
        public static unsafe int dotProductNeon(short* inputArray1, short* inputArray2, short len)
        {
            if (Neon.IsNeonSupported)
            {
                const int elementsPerIteration = 4;
                int iterations = len / elementsPerIteration;

                // 4-element vector of zeroes to accumulate the result
                var partialSumsNeon = new v128();

                // Main loop
                for (int i = 0; i < iterations; ++i)
                {
                    // Load vector elements to registers
                    var v1 = *(v64*)inputArray1;
                    var v2 = *(v64*)inputArray2;

                    partialSumsNeon = Neon.vmlal_s16(partialSumsNeon, v1, v2);

                    inputArray1 += elementsPerIteration;
                    inputArray2 += elementsPerIteration;
                }

                // Armv8 instruction to sum up all the elements into a single scalar
                int result = Neon.vaddvq_s32(partialSumsNeon);

                // Calculate the tail
                int tailLength = len % elementsPerIteration;
                while (tailLength-- > 0)
                {
                    result += *inputArray1 * *inputArray2;
                    inputArray1++;
                    inputArray2++;
                }

                return result;
            }
            else
                throw new System.NotSupportedException("Neon not supported");
        }
    }
}
