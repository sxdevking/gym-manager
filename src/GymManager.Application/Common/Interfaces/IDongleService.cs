using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManager.Application.Common.Interfaces;

/// <summary>
/// Servicio de comunicación con dongle USB
/// </summary>
public interface IDongleService
{
    /// <summary>
    /// Verifica si hay un dongle conectado
    /// </summary>
    bool IsDongleConnected();

    /// <summary>
    /// Obtiene el ID único del hardware del dongle
    /// </summary>
    string? GetHardwareId();

    /// <summary>
    /// Lee datos almacenados en el dongle
    /// </summary>
    byte[]? ReadFromDongle();

    /// <summary>
    /// Escribe datos en el dongle (solo para activación)
    /// </summary>
    bool WriteToDongle(byte[] data);

    /// <summary>
    /// Evento cuando se conecta/desconecta el dongle
    /// </summary>
    event EventHandler<bool>? DongleConnectionChanged;
}
