using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class LabelDateController : MonoBehaviour {

	public Text dateLabel;

	public LabelDisplay labelDisplay = LabelDisplay.UsFormat;

	// Change to other string if you like, including blank ' '
	public string dateDelim = "/"; // usually for US, UK etc
//	public string delim = "."; // for Germany etc

	public bool addZero = true;

	public string saveDateName = "LabelDate";

	public enum LabelDisplay {
		UsFormat,      // 12/01/2015  
		EuFormat,      // 01/12/2015
		UsFormatFull,  // December/01/2015
		EuFormatFull   // 01.December.2015
	}

	DateTime dateTime;

	// Use this for initialization
	void Start () {
//		clear save date if required
//		PlayerPrefs.SetString( saveDateName, "" );

		LoadFormat();

		SetFormat ();
	}

	public void LoadFormat () {
		string savedString = PlayerPrefs.GetString( saveDateName );
		Debug.Log( "LoadFormat: " + savedString );

		if( savedString.Equals("") ) {
			dateTime = System.DateTime.Now;
		}
		else {
			dateTime = DateTime.Parse( savedString );
		}
	}

	public void SetFormat () {

		if( labelDisplay == LabelDisplay.UsFormat ) {
			string year = dateTime.Year.ToString();
			string day = dateTime.Day.ToString();
			day = CalendarUtils.GetDayDigit( day, addZero);
			string month = dateTime.ToString("MM");  // 03
//			if( addZero && month.Length == 1 ) month = "0" + day; 
			month = CalendarUtils.GetMonthDigit( month, addZero);

			dateLabel.text = month + dateDelim + day + dateDelim + year;
		}
		if( labelDisplay == LabelDisplay.EuFormat ) {
			string year = dateTime.Year.ToString();
			string day = dateTime.Day.ToString();
			day = CalendarUtils.GetDayDigit( day, addZero);
			string month = dateTime.ToString("MM");  // 03
			month = CalendarUtils.GetMonthDigit( month, addZero);
//			if( addZero && month.Length == 1 ) month = "0" + month; 
			dateLabel.text = day + dateDelim + month + dateDelim + year;
		}
		if( labelDisplay == LabelDisplay.UsFormatFull ) {
			string year = dateTime.Year.ToString();
			string day = dateTime.Day.ToString();
			day = CalendarUtils.GetDayDigit( day, addZero);
			string month = dateTime.ToString("MMMM");  // 03
			dateLabel.text = month + dateDelim + day + dateDelim + year;
		}
		if( labelDisplay == LabelDisplay.EuFormatFull ) {
			string year = dateTime.Year.ToString();
			string day = dateTime.Day.ToString();
			day = CalendarUtils.GetDayDigit( day, addZero);
			string month = dateTime.ToString("MMMM");  // 03
			dateLabel.text = day + dateDelim + month + dateDelim + year;
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
