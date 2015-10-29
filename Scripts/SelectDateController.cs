using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class SelectDateController : MonoBehaviour {

	public InputField dayInput;
	public InputField monthInput;
	public InputField yearInput;

	public int monthNumber;

	public MonthDisplay monthDisplay = MonthDisplay.NumberSingle;

	public enum MonthDisplay {
		NumberSingle,
		NumberFirstZero,
		FirstThreeLetters,
		Full
	}

	DateTime dateTime;

	// Use this for initialization
	void Start () {
		dateTime = System.DateTime.Now;
		string year = dateTime.Year.ToString();
		string day = dateTime.Day.ToString();

		SetMonthFormat ();

		if( dayInput.text.Contains("") ) dayInput.text = day;
		if( yearInput.text.Contains("") ) yearInput.text = year;
	}


	public void SetMonthFormat () {

		string month = "";
		int monthInt = int.Parse( dateTime.ToString("MM") );  // 03
		monthNumber = monthInt;

		if( monthDisplay == MonthDisplay.NumberSingle ) {
			month = monthInt.ToString();
		}
		if( monthDisplay == MonthDisplay.NumberFirstZero ) {
			month = dateTime.ToString("MM");  // 03
		}
		if( monthDisplay == MonthDisplay.FirstThreeLetters ) {
			month = dateTime.ToString("MMM");  // Mar
		}
		if( monthDisplay == MonthDisplay.Full ) {
			month = dateTime.ToString("MMMM");  // March
		}
//		if( monthInput.text.Contains("") ) 
			monthInput.text = month;

		dateTime.AddMonths( monthInt );
	}

	public void CycleMonthFormat () {
//		int len = Enum.GetValues(typeof(MonthDisplay)).Length;
//		int i = 0;
//		foreach(MonthDisplay value in Enum.GetValues(typeof(MonthDisplay)))
//		{
//			print( value.ToString() );	
////			Array v = Enum.GetValues(typeof(MonthDisplay)) as Array;
////			print( v[i].ToString() );			
////			if( monthDisplay == value ) {
////				int idx = i+1;
////				if( i == len-1) idx = 0;
//////				v = Enum.GetValues(typeof(MonthDisplay))[idx];
////				monthDisplay = value; //v[idx];
////				break;
////			}
//			monthDisplay = value;
//			i++;
//		}
//
////		foreach( string item in System.Enum.GetNames(typeof(MonthDisplay) ) ) {
////			print( item );
////
////
////			monthDisplay = System.Enum.Parse( System.Type(MonthDisplay), item ); //. item.ToString();
////		}
//		SetMonthFormat ();
	}

	public void NextYear () {
		string y = yearInput.text.ToString();
		int year = int.Parse( y );
		year++;
		yearInput.text = year.ToString();
	}
	
	public void PrevYear () {
		string y = yearInput.text.ToString();
		int year = int.Parse( y );
		year--;
		yearInput.text = year.ToString();
	}

	// cycles through the months and starts from end or begin
	public bool cycleThroughMonth = false;

	public void NextMonth () {
		string month = "";
		monthNumber++;

		if( monthNumber > 12 ) {
			if( cycleThroughMonth ) monthNumber = 1; 
			else monthNumber = 12; 
		}

		month = CalendarUtils.GetMonthFull( monthNumber ); 
		monthInput.text = month;

		print( "NextMonth: " + monthNumber );
	}
	
	public void PrevMonth () {
		string month = "";
		monthNumber--;

		if( monthNumber < 1 ) {
			if( cycleThroughMonth ) if( monthNumber < 1 ) monthNumber = 12; 
			else monthNumber = 1; 
		}

		if( monthNumber < 1 ) { 
			monthNumber = 1; 
		}

		month = CalendarUtils.GetMonthFull( monthNumber ); 
		monthInput.text = month;
		print( "PrevMonth: " + monthNumber );
	}

	public void NextDay () {
		string d = dayInput.text.ToString();
		int day = int.Parse( d );
		day++;
		if( day > 31 ) day = 31; 
		dayInput.text = day.ToString();
	}
	
	public void PrevDay () {
		string d = dayInput.text.ToString();
		int day = int.Parse( d );
		day--;
		if( day < 1 ) day = 1; 
		dayInput.text = day.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
