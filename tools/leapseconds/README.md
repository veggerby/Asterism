# Leap Second Table Maintenance

This directory contains guidance and a helper script to update the built-in leap second table in `BuiltInLeapSecondProvider`.

## Source of Truth

IERS Bulletin C announcements (<https://www.iers.org/>) define when new leap seconds are inserted. Each inserted leap second takes effect at 23:59:60 UTC (i.e., the offset change applies starting the following day at 00:00:00 UTC).

## Built-In Table Format

The table in `BuiltInLeapSecondProvider` is an ordered array of `(DateTime dateUtc, int total)` entries where:

- `dateUtc` is the UTC instant (00:00:00) of the day the new cumulative TAI−UTC value becomes effective.
- `total` is the cumulative TAI−UTC seconds after the insertion.

## Update Steps

1. Obtain the latest Bulletin C. If it contains a new leap second, determine the effective date (the day AFTER the announced 23:59:60 insertion).
2. Append a new tuple to the array in `BuiltInLeapSecondProvider` keeping chronological order.
3. Increment the cumulative total by 1.
4. Update the `Source` string to include the new effective date.
5. Update `src/Asterism.Time/CHANGELOG.md` under `[Unreleased]` with an Added/Changed entry noting the new leap second.
6. Consider resetting `LeapSeconds.StalenessHorizonYears` default if horizon policy changes.
7. Run tests and add/adjust a test verifying the new boundary offset.

## CSV Provider Generation

Use `scripts/generate_leap_seconds_csv.sh` to create a CSV file consumable by `LeapSecondFileProvider` for validation/testing.

## Example New Entry

If a leap second is announced for 2026-12-31, add:

```csharp
(new System.DateTime(2027, 01, 01, 0,0,0, System.DateTimeKind.Utc), 38),
```

(assuming previous total 37)

## Verification Checklist

- [ ] Table entry appended and chronologically ordered.
- [ ] New total matches previous total + 1.
- [ ] Changelog updated.
- [ ] Tests updated (boundary before/after new effective date).
- [ ] Source string updated.

## Script Notes

The helper script does not fetch Bulletins automatically (offline deterministic policy). Manual review is required.
