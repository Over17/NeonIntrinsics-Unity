using System.Diagnostics;
using System.Text;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UI;

public class ResultDisplay : MonoBehaviour
{
    public Text m_Text;

    void Start()
    {
        // Ramp length and number of trials
        const int rampLength = 1027;
        const int trials = 1000000;

        m_Text.text = DoCalc(rampLength, trials);
    }

    private unsafe string DoCalc(short rampLength, int trials)
    {
        var sb = new StringBuilder();

        // Generate two input vectors
        // (0, 1, ..., rampLength - 1)
        // (100, 101, ..., 100 + rampLength-1)
        var ramp1 = generateRamp(0, rampLength);
        var ramp2 = generateRamp(100, rampLength);
        var noBurstPerfMarker = new ProfilerMarker("Not Bursted");
        var burstPerfMarker = new ProfilerMarker("Bursted");

        fixed (short* ramp1ptr = ramp1, ramp2ptr = ramp2)
        {
            int lastResult = 0;
            var timer = new Stopwatch();

            sb.AppendLine("----==== NO NEON ====----");
            noBurstPerfMarker.Begin();
            timer.Restart();
            lastResult = CalculateDotProd.dotProductScalar(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            noBurstPerfMarker.End();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            // Warm-up. First time running bursted version is slower than the subsequent times
            burstPerfMarker.Begin();
            timer.Restart();
            CalculateDotProd.dotProductBurst(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            burstPerfMarker.End();
            long first = timer.ElapsedMilliseconds;

            sb.AppendLine("----==== NO NEON, Bursted ====----");
            burstPerfMarker.Begin();
            timer.Restart();
            lastResult = CalculateDotProd.dotProductBurst(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            burstPerfMarker.End();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {first}, then {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, no unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, 2x unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon2(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, 3x unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon3(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, 4x unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon4(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, 6x unrolling ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon6(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== NEON, SMLAL+SMLAL2 2-wide ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon_with_SMLAL2_2wide(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();

            sb.AppendLine("----==== SMLAL+SMLAL2 4-wide ====----");
            timer.Restart();
            lastResult = CalculateDotProd.dotProductNeon_with_SMLAL2_4wide(ramp1ptr, ramp2ptr, rampLength, trials);
            timer.Stop();
            sb.AppendLine($"Result: {lastResult}")
                .AppendLine($"elapsedMs time: {timer.ElapsedMilliseconds} ms").AppendLine();
        }

        return sb.ToString();
    }

    static short[] generateRamp(short startValue, short len)
    {
        var ramp = new short[len];

        for (short i = 0; i < len; i++)
        {
            ramp[i] = (short)(startValue + i);
        }

        return ramp;
    }
}
