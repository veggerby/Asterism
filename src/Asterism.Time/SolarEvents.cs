namespace Asterism.Time;

/// <summary>
/// Contains the times of key solar events for a given date and location.
/// All times are expressed as <see cref="DateTimeOffset"/> in the specified time zone.
/// </summary>
/// <param name="Sunrise">The time of sunrise (upper limb of sun crosses horizon).</param>
/// <param name="SolarNoon">The time when the sun reaches its highest point in the sky (solar culmination).</param>
/// <param name="Sunset">The time of sunset (upper limb of sun crosses horizon).</param>
/// <param name="CivilDawn">The time when civil twilight begins (sun 6° below horizon). May be null in polar regions.</param>
/// <param name="CivilDusk">The time when civil twilight ends (sun 6° below horizon). May be null in polar regions.</param>
/// <param name="NauticalDawn">The time when nautical twilight begins (sun 12° below horizon). May be null in polar regions.</param>
/// <param name="NauticalDusk">The time when nautical twilight ends (sun 12° below horizon). May be null in polar regions.</param>
/// <param name="AstronomicalDawn">The time when astronomical twilight begins (sun 18° below horizon). May be null in polar regions.</param>
/// <param name="AstronomicalDusk">The time when astronomical twilight ends (sun 18° below horizon). May be null in polar regions.</param>
public record SolarEvents(
    DateTimeOffset? Sunrise,
    DateTimeOffset SolarNoon,
    DateTimeOffset? Sunset,
    DateTimeOffset? CivilDawn = null,
    DateTimeOffset? CivilDusk = null,
    DateTimeOffset? NauticalDawn = null,
    DateTimeOffset? NauticalDusk = null,
    DateTimeOffset? AstronomicalDawn = null,
    DateTimeOffset? AstronomicalDusk = null);