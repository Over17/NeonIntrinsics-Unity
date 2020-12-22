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

        [BurstCompile]
        public static unsafe int dotProductNeon2(short* inputArray1, short* inputArray2, short len)
        {
            if (Neon.IsNeonSupported)
            {
                const int elementsPerIteration = 8;
                int iterations = len / elementsPerIteration;

                // 4-element vectors of zeroes to accumulate partial results within the unrolled loop
                var partialSum1 = new v128();
                var partialSum2 = new v128();

                // Main loop, unrolled 2-wide
                for (int i = 0; i < iterations; ++i)
                {
                    // Load vector elements to registers
                    var v11 = *(v64*)inputArray1;
                    var v12 = *(v64*)(inputArray1 + 4);
                    var v21 = *(v64*)inputArray2;
                    var v22 = *(v64*)(inputArray2 + 4);

                    partialSum1 = Neon.vmlal_s16(partialSum1, v11, v21);
                    partialSum2 = Neon.vmlal_s16(partialSum2, v12, v22);

                    inputArray1 += elementsPerIteration;
                    inputArray2 += elementsPerIteration;
                }
                // Now sum up the results of the 2 partial sums from the loop
                var partialSumsNeon = Neon.vaddq_s32(partialSum1, partialSum2);

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

        [BurstCompile]
        public static unsafe int dotProductNeon3(short* inputArray1, short* inputArray2, short len)
        {
            if (Neon.IsNeonSupported)
            {
                const int elementsPerIteration = 12;
                int iterations = len / elementsPerIteration;

                // 4-element vectors of zeroes to accumulate partial results within the unrolled loop
                var partialSum1 = new v128();
                var partialSum2 = new v128();
                var partialSum3 = new v128();

                // Main loop, unrolled 3-wide
                for (int i = 0; i < iterations; ++i)
                {
                    // Load vector elements to registers
                    var v11 = *(v64*)inputArray1;
                    var v12 = *(v64*)(inputArray1 + 4);
                    var v13 = *(v64*)(inputArray1 + 8);
                    var v21 = *(v64*)inputArray2;
                    var v22 = *(v64*)(inputArray2 + 4);
                    var v23 = *(v64*)(inputArray2 + 8);

                    partialSum1 = Neon.vmlal_s16(partialSum1, v11, v21);
                    partialSum2 = Neon.vmlal_s16(partialSum2, v12, v22);
                    partialSum3 = Neon.vmlal_s16(partialSum3, v13, v23);

                    inputArray1 += elementsPerIteration;
                    inputArray2 += elementsPerIteration;
                }
                // Now sum up the results of the 3 partial sums from the loop
                var partialSumsNeon = Neon.vaddq_s32(partialSum1, partialSum2);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum3);

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

        [BurstCompile]
        public static unsafe int dotProductNeon4(short* inputArray1, short* inputArray2, short len)
        {
            if (Neon.IsNeonSupported)
            {
                const int elementsPerIteration = 16;
                int iterations = len / elementsPerIteration;

                // 4-element vectors of zeroes to accumulate partial results within the unrolled loop
                var partialSum1 = new v128();
                var partialSum2 = new v128();
                var partialSum3 = new v128();
                var partialSum4 = new v128();

                // Main loop, unrolled 4-wide
                for (int i = 0; i < iterations; ++i)
                {
                    // Load vector elements to registers
                    var v11 = *(v64*)inputArray1;
                    var v12 = *(v64*)(inputArray1 + 4);
                    var v13 = *(v64*)(inputArray1 + 8);
                    var v14 = *(v64*)(inputArray1 + 12);
                    var v21 = *(v64*)inputArray2;
                    var v22 = *(v64*)(inputArray2 + 4);
                    var v23 = *(v64*)(inputArray2 + 8);
                    var v24 = *(v64*)(inputArray2 + 12);

                    partialSum1 = Neon.vmlal_s16(partialSum1, v11, v21);
                    partialSum2 = Neon.vmlal_s16(partialSum2, v12, v22);
                    partialSum3 = Neon.vmlal_s16(partialSum3, v13, v23);
                    partialSum4 = Neon.vmlal_s16(partialSum4, v14, v24);

                    inputArray1 += elementsPerIteration;
                    inputArray2 += elementsPerIteration;
                }
                // Now sum up the results of the 3 partial sums from the loop
                var partialSumsNeon = Neon.vaddq_s32(partialSum1, partialSum2);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum3);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum4);

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

        [BurstCompile]
        public static unsafe int dotProductNeon6(short* inputArray1, short* inputArray2, short len)
        {
            if (Neon.IsNeonSupported)
            {
                const int elementsPerIteration = 24;
                int iterations = len / elementsPerIteration;

                // 4-element vectors of zeroes to accumulate partial results within the unrolled loop
                var partialSum1 = new v128();
                var partialSum2 = new v128();
                var partialSum3 = new v128();
                var partialSum4 = new v128();
                var partialSum5 = new v128();
                var partialSum6 = new v128();

                // Main loop, unrolled 4-wide
                for (int i = 0; i < iterations; ++i)
                {
                    // Load vector elements to registers
                    var v11 = *(v64*)inputArray1;
                    var v12 = *(v64*)(inputArray1 + 4);
                    var v13 = *(v64*)(inputArray1 + 8);
                    var v14 = *(v64*)(inputArray1 + 12);
                    var v15 = *(v64*)(inputArray1 + 16);
                    var v16 = *(v64*)(inputArray1 + 20);
                    var v21 = *(v64*)inputArray2;
                    var v22 = *(v64*)(inputArray2 + 4);
                    var v23 = *(v64*)(inputArray2 + 8);
                    var v24 = *(v64*)(inputArray2 + 12);
                    var v25 = *(v64*)(inputArray2 + 16);
                    var v26 = *(v64*)(inputArray2 + 20);

                    partialSum1 = Neon.vmlal_s16(partialSum1, v11, v21);
                    partialSum2 = Neon.vmlal_s16(partialSum2, v12, v22);
                    partialSum3 = Neon.vmlal_s16(partialSum3, v13, v23);
                    partialSum4 = Neon.vmlal_s16(partialSum4, v14, v24);
                    partialSum5 = Neon.vmlal_s16(partialSum5, v15, v25);
                    partialSum6 = Neon.vmlal_s16(partialSum6, v16, v26);

                    inputArray1 += elementsPerIteration;
                    inputArray2 += elementsPerIteration;
                }
                // Now sum up the results of the 6 partial sums from the loop
                var partialSumsNeon = Neon.vaddq_s32(partialSum1, partialSum2);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum3);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum4);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum5);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum6);

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

        [BurstCompile]
        public static unsafe int dotProductNeon_with_SMLAL2_2wide(short* inputArray1, short* inputArray2, short len)
        {
            if (Neon.IsNeonSupported)
            {
                const int elementsPerIteration = 8;
                int iterations = len / elementsPerIteration;

                // 4-element vectors of zeroes to accumulate partial results within the unrolled loop
                var partialSumLow = new v128();
                var partialSumHigh = new v128();

                // Main loop, unrolled 2-wide
                for (int i = 0; i < iterations; ++i)
                {
                    // Load vector elements to registers
                    // Comparing to SMLAL variant, we're loading 128-bit vectors here (8x short int)
                    // and using SMLAL2 (vmlal_high_s16) to calculate dot product for the upper half
                    // This way, we're doing only half of the loads comparing to dotProductNeon2
                    var v1 = *(v128*)inputArray1;
                    var v2 = *(v128*)inputArray2;

                    partialSumLow = Neon.vmlal_s16(partialSumLow, v1.Lo64, v2.Lo64);
                    partialSumHigh = Neon.vmlal_high_s16(partialSumHigh, v1, v2);

                    inputArray1 += elementsPerIteration;
                    inputArray2 += elementsPerIteration;
                }
                // Now sum up the results of the 2 partial sums from the loop
                var partialSumsNeon = Neon.vaddq_s32(partialSumLow, partialSumHigh);

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

        [BurstCompile]
        public static unsafe int dotProductNeon_with_SMLAL2_4wide(short* inputArray1, short* inputArray2, short len)
        {
            if (Neon.IsNeonSupported)
            {
                const int elementsPerIteration = 16;
                int iterations = len / elementsPerIteration;

                // 4-element vectors of zeroes to accumulate partial results within the unrolled loop
                var partialSum1Low = new v128();
                var partialSum1High = new v128();
                var partialSum2Low = new v128();
                var partialSum2High = new v128();

                // Main loop, unrolled 4-wide
                for (int i = 0; i < iterations; ++i)
                {
                    // Load vector elements to registers
                    // Comparing to SMLAL variant, we're loading 128-bit vectors here (8x short int)
                    // and using SMLAL2 (vmlal_high_s16) to calculate dot product for the upper half
                    // This way, we're doing only half of the loads comparing to dotProductNeon2
                    var v11 = *(v128*)inputArray1;
                    var v12 = *(v128*)(inputArray1 + 8);
                    var v21 = *(v128*)inputArray2;
                    var v22 = *(v128*)(inputArray2 + 8);

                    partialSum1Low = Neon.vmlal_s16(partialSum1Low, v11.Lo64, v21.Lo64);
                    partialSum1High = Neon.vmlal_high_s16(partialSum1High, v11, v21);
                    partialSum2Low = Neon.vmlal_s16(partialSum2Low, v12.Lo64, v22.Lo64);
                    partialSum2High = Neon.vmlal_high_s16(partialSum2High, v12, v22);

                    inputArray1 += elementsPerIteration;
                    inputArray2 += elementsPerIteration;
                }
                // Now sum up the results of the 2 partial sums from the loop
                var partialSumsNeon = Neon.vaddq_s32(partialSum1Low, partialSum1High);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum2Low);
                partialSumsNeon = Neon.vaddq_s32(partialSumsNeon, partialSum2High);

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
