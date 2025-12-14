using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ImageCompressor.Web.Data;

public class SystemMetrics
{
    public string OsDescription { get; set; } = "";
    public string HostName { get; set; } = "";
    public int ProcessorCount { get; set; }

    // RAM Verileri
    public long MemoryUsed { get; set; }
    public long TotalMemory { get; set; } // Toplam Limit (Konteyner Limiti)
    public double MemoryUsagePercent { get; set; }

    // CPU Verileri
    public double CpuUsagePercent { get; set; }

    public TimeSpan Uptime { get; set; }
    public int ThreadCount { get; set; }
}

public class SystemMonitorService
{
    // CPU hesaplaması için önceki zamanı hafızada tutmamız lazım
    private TimeSpan _prevCpuTime = TimeSpan.Zero;
    private DateTime _prevTime = DateTime.MinValue;

    public SystemMetrics GetMetrics()
    {
        var process = Process.GetCurrentProcess();

        // 1. RAM Hesaplaması
        var usedMem = process.WorkingSet64;
        // Docker konteynerinin toplam bellek limitini alır (yoksa fiziksel RAM'i alır)
        var totalMem = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;

        double memPercent = 0;
        if (totalMem > 0)
        {
            memPercent = ((double)usedMem / totalMem) * 100;
        }

        // 2. CPU Hesaplaması (Delta Time Yöntemi)
        var currentCpuTime = process.TotalProcessorTime;
        var currentTime = DateTime.UtcNow;
        double cpuPercent = 0;

        if (_prevTime != DateTime.MinValue)
        {
            var cpuTimeDiff = (currentCpuTime - _prevCpuTime).TotalMilliseconds;
            var timeDiff = (currentTime - _prevTime).TotalMilliseconds;

            if (timeDiff > 0)
            {
                // (Kullanılan CPU Süresi) / (Geçen Gerçek Süre * Çekirdek Sayısı)
                cpuPercent = (cpuTimeDiff / (timeDiff * Environment.ProcessorCount)) * 100;
            }
        }

        // Değerleri bir sonraki hesaplama için sakla
        _prevCpuTime = currentCpuTime;
        _prevTime = currentTime;

        return new SystemMetrics
        {
            OsDescription = RuntimeInformation.OSDescription + " (" + RuntimeInformation.OSArchitecture + ")",
            HostName = System.Net.Dns.GetHostName(),
            ProcessorCount = Environment.ProcessorCount,

            MemoryUsed = usedMem,
            TotalMemory = totalMem,
            MemoryUsagePercent = Math.Min(100, Math.Max(0, memPercent)), // 0-100 arasına sabitle

            CpuUsagePercent = Math.Min(100, Math.Max(0, cpuPercent)), // 0-100 arasına sabitle

            Uptime = DateTime.Now - process.StartTime,
            ThreadCount = process.Threads.Count
        };
    }
}