using System.Globalization;
using System.Net.Sockets;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/wakeup", (string macAddress, string ipAddress, int port) =>
{
    var macBytes = macAddress.Split('-')
                      .Select(x => byte.Parse(x, NumberStyles.HexNumber))
                      .ToArray();

    // ���� WOL ħ����
    var packet = new byte[102];
    for (int i = 0; i < 6; i++)
        packet[i] = 0xFF;
    for (int i = 6; i < 102; i += 6)
        macBytes.CopyTo(packet, i);

    // ��Ŀ���豸���� WOL ħ����
    using (var client = new UdpClient())
    {
        client.Connect(IPAddress.Parse(ipAddress), port);
        return client.Send(packet, packet.Length);
    }
})
.WithName("wakeup");

app.Run();