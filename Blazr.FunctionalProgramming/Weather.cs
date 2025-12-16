using System;
using System.Collections.Generic;
using System.Text;

namespace Blazr.FunctionalProgramming;

public class WeatherForecast1
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateOnly Date { get; set; }
    public decimal TemperatureC { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public record WeatherForecast (
    Guid Id,
    DateOnly Date ,
    decimal TemperatureC,
    string Summary
    );

public class WeatherForecastMutor
{
    public WeatherForecast BaseRecord { get; private set; }
    public DateOnly Date { get; set; }
    public decimal TemperatureC { get; set; }
    public string Summary { get; set; } = string.Empty;
    public bool IsNew { get; private init; }

    public WeatherForecastMutor(WeatherForecast record )
    {
        this.BaseRecord = record;
        this.Set();
    }
    public WeatherForecastMutor()
    {
        this.BaseRecord = new(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Now), 0, "[Not Set]");
        this.Set();
        this.IsNew = true;
    }

    private void Set()
    {
        this.Date = this.BaseRecord.Date;
        this.Summary = this.BaseRecord.Summary;
        this.TemperatureC = this.BaseRecord.TemperatureC;    
    }

    public WeatherForecast Record
        => new WeatherForecast(
            Id: this.BaseRecord.Id,
            Date: this.Date,
            TemperatureC: this.TemperatureC,
            Summary: this.Summary
            );

    public bool IsDirty
        => this.Record != this.BaseRecord;
}
