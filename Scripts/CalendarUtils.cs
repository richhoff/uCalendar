using UnityEngine;
using System.Collections;

public class CalendarUtils {

	// GetMonthDigit and GetMonthDigitInt use this to convert month text to int 
	public static string[] monthList = new string[12] {"January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

	public static string GetDayDigit ( string day, bool doubleDigit ) {
		string ret = day;
		if( doubleDigit && day.Length < 2 ) ret = "0" + ret;
//		Debug.Log( "GetDayDigit: " + day + " ret: " + ret );
		return ret;
	}

	// get text month to digit in string, ONLY IN ENGLISH for now!!! Change monthList param for your language!
	public static string GetMonthDigit ( string month, bool doubleDigit ) {
		string ret = "";
		int monthInt = 1;
		if( month.Length <= 2 ) {
			monthInt = int.Parse( month );
		}
		else {
			monthInt = GetMonthDigitInt( month );
		}
		ret = monthInt + "";
		if( doubleDigit && monthInt < 10 ) ret = "0" + ret;
		//		Debug.Log( "GetMonthDigit: " + month + " ret: " + ret );
		return ret;
	}
	
	// get text month to digit in int
	public static int GetMonthDigitInt ( string month ) {
		int ret = 1;
		for( int i = 0; i < 12; i++ ) {
			if( month.Contains( monthList[i] ) ) {
				ret = i+1;
				break;
			}
		}
//		Debug.Log( "GetMonthDigitInt: " + month + " ret: " + ret );
		return ret; // fail save
	}

	public static string GetMonthFull ( int month ) {
		int m = month-1;
		if( m < 0 ) m = 0;
		return monthList[m];
	}

}
