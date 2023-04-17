using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    // Settings
    [SerializeField] private float timeScale = 1f;
    private float tickPeriod = 0.200f; // In real time seconds
    private int ticksPerSecond = 5; // Game time vv
    private int secondsPerMinute = 60;
    private int minutesPerHour = 60;
    private int hoursPerDay = 24;
    private int daysPerWeek = 7;
    private int weeksPerMonth = 4;
    private int monthsPerSeason = 3;
    private int monthsPerYear = 12;
    public enum Season {
        Summer = 1,
        Fall = 2,
        Winter = 3,
        Spring = 4
    }

    // Events
    public static event EventHandler OnTick;
    public static event EventHandler OnSecondChange;
    public static event EventHandler OnTenSeconds;
    public static event EventHandler OnMinuteChange;
    public static event EventHandler OnHourChange;
    public static event EventHandler OnDayChange;
    public static event EventHandler OnWeekChange;
    public static event EventHandler OnMonthChange;
    public static event EventHandler OnSeasonChange;
    public static event EventHandler OnYearChange;

    public class OnTimeChangeEventArgs : EventArgs {
        public int current;
    }

    public class TimeObject {
        public int second { get; private set; }
        public int minute { get; private set; }
        public int hour { get; private set; }
        public int day { get; private set; }
        public int week { get; private set; }
        public int month { get; private set; }
        public int season { get; private set; }
        public int year { get; private set; }

        public TimeObject(int _second, int _minute, int _hour, int _day, int _week, int _month, int _season, int _year) {
            second = _second;
            minute = _minute;
            hour = _hour;
            day = _day;
            week = _week;
            month = _month;
            season = _season;
            year = _year;
        }
    }


    // States
    private TimeObject loadTime = null;
    private float tickTimer = 0f;
    public int currentTick { get; private set; }
    public int currentSecond { get; private set; } = 1;
    public int currentMinute { get; private set; } = 1;
    public int currentHour { get; private set; } = 1;
    public int currentDay { get; private set; } = 1;
    public int currentWeek { get; private set; } = 1;
    public int currentMonth { get; private set; } = 1;
    public Season currentSeason { get; private set; } = Season.Summer;
    public int currentYear { get; private set; } = 1;

    void Start()
    {
        if (loadTime != null) {
            SetTime(loadTime);
        }
    }

    void Update()
    {
        ProgressTime();

        // transform.Rotate(new Vector3(timeScale * Time.deltaTime, 0, 0));
    }

    public void LoadTime(TimeObject _time) {
        loadTime = _time;
    }

    void SetTime(TimeObject _time) {
        currentSecond = loadTime.second;
        currentMinute = loadTime.minute;
        currentHour = loadTime.hour;
        currentDay = loadTime.day;
        currentWeek = loadTime.week;
        currentMonth = loadTime.month;
        currentSeason = (Season)loadTime.season;
        currentYear = loadTime.year;

        loadTime = null;
    }

    void ProgressTime() {
        tickTimer += Time.deltaTime * timeScale;

        if (tickTimer >= tickPeriod) {
            tickTimer -= tickPeriod;
            ProgressTicks();
        }
    }

    void ProgressTicks() {
        currentTick++;
        OnTick?.Invoke(this, EventArgs.Empty);

        if (currentTick >= ticksPerSecond) {
            currentTick -= ticksPerSecond;
            ProgressSeconds();
        }
    }

    void ProgressSeconds() {
        currentSecond++;
        OnSecondChange?.Invoke(this, new OnTimeChangeEventArgs{ current = currentSecond });

        if (currentSecond % 10 == 0) {
            OnTenSeconds?.Invoke(this, EventArgs.Empty);
        }

        if (currentSecond >= secondsPerMinute + 1)  {
            currentSecond -= secondsPerMinute;
            ProgressMinutes();
        }
    }

    void ProgressMinutes() {
        currentMinute++;
        OnMinuteChange?.Invoke(this, new OnTimeChangeEventArgs{ current = currentMinute });

        if (currentMinute >= minutesPerHour + 1)  {
            currentMinute -= minutesPerHour;
            ProgressHours();
        }
    }

    void ProgressHours() {
        currentHour++;
        OnHourChange?.Invoke(this, new OnTimeChangeEventArgs{ current = currentHour });

        if (currentHour >= hoursPerDay + 1)  {
            currentHour -= hoursPerDay;
            ProgressDays();
        }
    }

    void ProgressDays() {
        currentDay++;
        OnDayChange?.Invoke(this, new OnTimeChangeEventArgs{ current = currentDay });

        if (currentDay >= daysPerWeek + 1)  {
            currentDay -= daysPerWeek;
            ProgressWeeks();
        }
    }

    void ProgressWeeks() {
        currentWeek++;
        OnWeekChange?.Invoke(this, new OnTimeChangeEventArgs{ current = currentWeek });

        if (currentWeek >= weeksPerMonth + 1)  {
            currentWeek -= weeksPerMonth;
            ProgressMonths();
        }
    }

    void ProgressMonths() {
        currentMonth++;
        OnMonthChange?.Invoke(this, new OnTimeChangeEventArgs{ current = currentMonth });

        if (currentMonth % monthsPerSeason == 0)  {
            ProgressSeasons();
        }

        if (currentMonth >= monthsPerYear + 1)  {
            currentMonth -= monthsPerYear;
            ProgressYears();
        }
    }

    void ProgressSeasons() {
        currentSeason++;
        if ((int)currentSeason > 4) {
            currentSeason = Season.Summer;
        }
        OnSeasonChange?.Invoke(this, new OnTimeChangeEventArgs{ current = (int)currentSeason });
    }

    void ProgressYears() {
        currentYear++;
        OnYearChange?.Invoke(this, new OnTimeChangeEventArgs{ current = currentYear });
    }

    public void GetCurrentTime() {
        Debug.Log($"The current time is {currentHour}:{currentMinute}:{currentSecond} on day {currentDay} of month {currentMonth} in year {currentYear}, during the {currentSeason} season.");
    }
}
