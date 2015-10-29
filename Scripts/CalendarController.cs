using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class CalendarController : MonoBehaviour {

	public GameObject canvasCalendar;
	public GameObject targetCalendarPanel;
	public GameObject contentCalDay;
	public GameObject targetTopWeekDaysPanel;
	public GameObject targetTopWeekDays;
	public GameObject targetMonthPanel;
	public GameObject contentMonth;

	public InputField yearInput;
	public Button monthButton;

	// callback target object that spawned the calendar
	public GameObject targetObject;
	public GameObject selectDayObject;

	public string[] listWeekDays = new string[] {"Mo","Tu","We","Tu","Fr","Sa","So"};

//	public List<string> listDays = new List<string>();
	public List<string> listMonths = new List<string>();
	public List<GameObject> listDayObject = new List<GameObject>();

	public Color currentDayColor = Color.yellow;
	public Color selectedDayColor = Color.red;
	public Color passiveDayColor = new Color( 245f, 245f, 245f );

	public Color prevMonthDayColor = Color.grey;
	public Color nextMonthDayColor = Color.grey;

	// Date delimiter, default is "/", German format for example is "."
	public string dateDelim =  "/";

	// Some countries like USA have month first and day second
	public bool usFormat = false;
	public bool addZero = true;
	public bool hideCalendarOnStart = true;
	public bool showWeekdaysFirstCharOnly = true;
	public bool closeCalendarAfterSelect = true;

//	public bool startWeekFromMonday = true;

	// player prefabs save name
	public string saveNameDate = "selected_date";

	// default: Depends on the Panel Calendar width and height, 
	// so be careful if you change that in the inspector or scene directly 
	public int rows = 5;
	public int cols = 7;

	int daysInMonth = 31;
	DateTime dateTimeNow;
	DateTime dateTime;
	string year = "";
	string month = "";
	string day = "";
	string monthDigit = "";
	string currentDay = "";
	string monthDigitSaved = "";
	string yearSaved = "";
	bool isSaveDate = false;

	// Use this for initialization
	void Start () {
		StartCalendar();
	}

	void StartCalendar () {

//		clear save date if required
//		PlayerPrefs.SetString( saveDateName, "" );

		if( canvasCalendar == null ) canvasCalendar = gameObject; // assign same gameObject if no canvas / panel manually assigned

		SetDateTimeNow();

		if( !targetCalendarPanel ) targetCalendarPanel = gameObject;
		contentCalDay.SetActive( false );
		contentMonth.SetActive( false );

		targetMonthPanel.SetActive( false );

//		Debug.Log( "Start monthDigit: " + monthDigit );
		SetCalendar();
		CreateMonthsDropDown();

		CreateWeekdays();

		if( hideCalendarOnStart ) canvasCalendar.SetActive( false );
	}

	void SetDateTimeNow () {
		dateTimeNow = System.DateTime.Now;
		
		dateTime = dateTimeNow; 
		currentDay = dateTime.Day.ToString();
		if( currentDay.Length == 1 ) currentDay = "0" + currentDay; 
		
		year = dateTime.Year.ToString();
		month = dateTime.ToString("MMMM");
		monthDigit = dateTime.ToString("MM"); // Digit number only
		day = dateTime.Day.ToString();
		if( day.Length == 1 ) day = "0" + day; 
		
		yearInput.text = year;
		SetSelectedMonth( month );
//		SetCalendar();
	}

	public void CreateWeekdays () {
		if( targetCalendarPanel != null ) {

			for( int i = 0; i < listWeekDays.Length; i++ ) {
				GameObject g = Instantiate( targetTopWeekDays ) as GameObject;
				g.SetActive( true );
				g.transform.SetParent( targetTopWeekDaysPanel.transform );
				string weekdayStr = listWeekDays[i];
				if( showWeekdaysFirstCharOnly ) {
					weekdayStr = weekdayStr.Substring(0,1);
				}
				g.name = weekdayStr;
				
				g.transform.GetChild(0).GetComponent<Text>().text = weekdayStr;
				g.transform.localScale = Vector3.one;
				g.transform.localPosition = Vector3.zero;
			}
			targetTopWeekDays.SetActive( false );
		}

	}

	// TODO: 
	public void CreateWeekNumbers () {
		
	}

	// create list for drop down select
	public void CreateMonthsDropDown () {

		if( targetCalendarPanel != null ) {

			for( int i = 0; i < 12; i++ ) {
				int m = i+1;
				DateTime date = new DateTime( dateTime.Year, m, 1 );
				string month = date.ToString("MMMM");
				listMonths.Add( month );

				monthDigit = date.ToString("MM");

				GameObject g = Instantiate( contentMonth ) as GameObject;
				g.SetActive( true );
				g.transform.SetParent( targetMonthPanel.transform );
				g.name = "" + month;
				
				g.transform.GetChild(0).GetComponent<Text>().text = month;
				g.transform.localScale = Vector3.one;
				g.transform.localPosition = Vector3.zero;
			}
		}
	}

	// create calendar for grid layout
	public void SetCalendar () {
		SetCalendar ( true );
	}

	public void SetCalendar ( bool loadData ) {

		if( targetCalendarPanel != null ) {

			// clean it up first
			listDayObject.Clear();
//			listDays.Clear();
			int childIdx = 0;
			foreach( Transform t in targetCalendarPanel.transform ) {
				if( childIdx > 0 ) {
					Destroy( t.gameObject );
				}
				childIdx++;
			}
//			ClearCalendar ();

			if( loadData ) LoadDate();

			monthButton.transform.GetChild(0).GetComponent<Text>().text = month;
			dateTimeNow = System.DateTime.Now;
			monthDigit = CalendarUtils.GetMonthDigit( month, true );
			daysInMonth = GetDaysInMonth( int.Parse( year ), int.Parse( monthDigit ) );

			// previous month 
			int fillPrevMonthDays = 0;
//			Debug.Log( "SetCalendar - monthDigit: " + monthDigit + ", year: " + year + " , daysInMonth: " + daysInMonth );
			DateTime dateTimeDay1 = DateTime.Parse( year + "/" + monthDigit  + "/" + 1 );
			if( !dateTimeDay1.DayOfWeek.Equals( "Monday" ) ) {
				int yearDigitPrev = int.Parse( year );
				int monthDigitPrev = int.Parse( monthDigit )-1;
				if( monthDigitPrev < 1 ) {
					monthDigitPrev = 12;
					yearDigitPrev = yearDigitPrev-1;
				}

//				Debug.Log ( "fillPrevMonthDays: " + dateTimeDay1.DayOfWeek  );
				if( dateTimeDay1.DayOfWeek.ToString().Equals("Tuesday"    ) ) fillPrevMonthDays = 1;
				else if( dateTimeDay1.DayOfWeek.ToString().Equals( "Wednesday" ) ) fillPrevMonthDays = 2;
				else if( dateTimeDay1.DayOfWeek.ToString().Equals( "Thursday"  ) ) fillPrevMonthDays = 3;
				else if( dateTimeDay1.DayOfWeek.ToString().Equals( "Friday"    ) ) fillPrevMonthDays = 4;
				else if( dateTimeDay1.DayOfWeek.ToString().Equals( "Saturday"  ) ) fillPrevMonthDays = 5;
				else if( dateTimeDay1.DayOfWeek.ToString().Equals( "Sunday"    ) ) fillPrevMonthDays = 6;

				int daysInMonthPrev = GetDaysInMonth( yearDigitPrev, monthDigitPrev );

//				int k = 0;
				for( int i = daysInMonthPrev-fillPrevMonthDays+1; i <= daysInMonthPrev; i++ ) {
					string dayString = i + "";
					DateTime dateTime = DateTime.Parse( yearDigitPrev + "/" + monthDigitPrev  + "/" + i );
//					dayString += " " + dateTime.DayOfWeek; //ToString("MM/dd/yyyy");;
//					listDays.Add( dayString );
					
					GameObject g = Instantiate( contentCalDay ) as GameObject;
					g.SetActive( true );
					g.transform.SetParent( targetCalendarPanel.transform);
					g.name = dateTime.ToString( "MM/dd/yyyy" );				
					g.transform.GetComponent<Image>().color = prevMonthDayColor;
					g.transform.localScale = Vector3.one;
					g.transform.localPosition = Vector3.zero;
//					g.transform.GetComponent<Button>().onClick.AddListener( // if is an external prefab
					listDayObject.Add( g );
					g.transform.GetChild(0).GetComponent<Text>().text = listDayObject[ listDayObject.Count-1 ].name.Split('/')[1];  // listDays[listDays.Count-1];
//					k++;
				}
			}

			// current month
			for( int i = 1; i <= daysInMonth; i++ ) {
				string dayString = i + "";
				DateTime dateTime = DateTime.Parse( year + "/" + monthDigit  + "/" + i );
//				dayString += " " + dateTime.DayOfWeek; //ToString("MM/dd/yyyy");;
//				listDays.Add( dayString );

				GameObject g = Instantiate( contentCalDay ) as GameObject;
				g.SetActive( true );
				g.transform.SetParent( targetCalendarPanel.transform);
				g.name = dateTime.ToString( "MM/dd/yyyy" );				
				g.transform.localScale = Vector3.one;
				g.transform.localPosition = Vector3.zero;
				SetDayColor( g, dayString );
				listDayObject.Add( g );
				g.transform.GetChild(0).GetComponent<Text>().text = listDayObject[ listDayObject.Count-1 ].name.Split('/')[1];  //listDays[listDays.Count-1];
			}

			// next month
			int monthDigitNext = int.Parse( monthDigit )+1;
			int yearDigitNext = int.Parse( year );
			if( monthDigitNext > 12 ) {
				monthDigitNext = 1;
				yearDigitNext = yearDigitNext+1;
			}

			int gridCount = rows * cols;

			int len = gridCount-listDayObject.Count; // listDays.Count;
			for( int i = 1; i <= len; i++ ) {
				string dayString = i + "";
				DateTime dateTime = DateTime.Parse( yearDigitNext + "/" + monthDigitNext  + "/" + i );
//				dayString += " " + dateTime.DayOfWeek; //ToString("MM/dd/yyyy");;
//				listDays.Add( dayString );
				
				GameObject g = Instantiate( contentCalDay ) as GameObject;
				g.SetActive( true );
				g.transform.SetParent( targetCalendarPanel.transform);
				g.name = dateTime.ToString( "MM/dd/yyyy" );				
//				g.transform.GetChild(0).GetComponent<Text>().text = dayString; //listDays[listDays.Count-1];
				g.transform.GetComponent<Image>().color = nextMonthDayColor;
				g.transform.localScale = Vector3.one;
				g.transform.localPosition = Vector3.zero;
				listDayObject.Add( g );
				g.transform.GetChild(0).GetComponent<Text>().text = listDayObject[ listDayObject.Count-1].name.Split('/')[1];  //listDays[listDays.Count-1];
			}
		}
	}

	public void ClearCalendar () {
		for( int i = 0; i < daysInMonth; i++ ) {
			Destroy( listDayObject[i] );
		}
		listDayObject.Clear();
//		listDays.Clear();
	}

	public void CloseCalendar () {
		canvasCalendar.SetActive( false );
	}

	// create calendar for grid layout
	public void UpdateCalendar () {
		
		if( targetCalendarPanel != null ) {
			SetCalendar( false );
			// updated just in case date is next day, month, year etc

			dateTimeNow = System.DateTime.Now;
//			string monthNowDigit = dateTimeNow.ToString("MM");

			SetDayColor();

//			listDays.Clear();
//			for( int i = 0; i < daysInMonth; i++ ) {
//				string dayString = (i+1) + "";
////				listDays.Add( dayString );
//
//				GameObject g = listDayObject[i] as GameObject;
////				Debug.Log ( "UpdateCalendar: " + dayString + " , dateTimeNow: " + dateTimeNow.ToString() );
//				SetDayColor( g, dayString );
//			}
		}
		
	}

	void SetDayColor ()
	{	
		string monthNowDigit = dateTimeNow.ToString("MM");
		string yearNow = dateTimeNow.Year.ToString();

//		listDays.Clear();
		for( int i = 0; i < daysInMonth; i++ ) {
			string dayString = (i+1) + "";
//			listDays.Add( dayString );

			GameObject gameObjectDay = listDayObject[i];

			if( selectDayObject != null ) {
				string [] split = selectDayObject.name.Split('/');
				// get the day if exists
				if( split.Length > 1 ) {
					day = split[1];
					if( dayString.Length == 1 ) dayString = "0" + dayString;

//					string searchDateString = monthNowDigit + "/" + dayString + "/" + yearNow; // dayString + "/" + monthNowDigit + "/" + yearNow;
//					GameObject gameObject = GameObject.Find( searchDateString );

			//		GameObject gameObjectSelected = GameObject.Find( selectDayObject.name );
			//		gameObjectSelected.transform.GetComponent<Image>().color = selectedDayColor;

					if( day.Equals( dayString ) && monthDigit.Equals( monthDigitSaved ) && year.Equals( yearNow ) ) {
						gameObjectDay.transform.GetComponent<Image>().color = selectedDayColor;
						Debug.Log ( "SetDayColor - selectedDay: " + dayString );
						continue;
					}
				}
			}

			if( currentDay.Equals( dayString ) && monthDigit.Equals( monthNowDigit ) && year.Equals( yearNow ) ) {
				if( gameObjectDay ) gameObjectDay.transform.GetComponent<Image>().color = currentDayColor;
				Debug.Log ( "SetDayColor - currentDay: " + currentDay );
			}
			else {
				if( gameObjectDay ) gameObjectDay.transform.GetComponent<Image>().color = passiveDayColor;
			}
		}
	}

	void SetDayColor ( GameObject gameObject, string dayString )
	{	
		if( dayString.Length == 1 ) dayString = "0" + dayString; 
		string monthNowDigit = dateTimeNow.ToString("MM");
		string yearNow = dateTimeNow.Year.ToString();

		if( day.Equals( dayString ) && monthDigit.Equals( monthDigitSaved ) && year.Equals( yearNow ) ) {
			gameObject.transform.GetComponent<Image>().color = selectedDayColor;
			Debug.Log ( "SetDayColor - selectedDay: " + dayString );
		}
		else if( currentDay.Equals( dayString ) && monthDigit.Equals( monthNowDigit ) && year.Equals( yearNow ) ) {
			gameObject.transform.GetComponent<Image>().color = currentDayColor;
			Debug.Log ( "SetDayColor - currentDay: " + currentDay );
		}
		else {
			gameObject.transform.GetComponent<Image>().color = passiveDayColor;
		}
	}

	// Next yesr button
	public void NextYear () {
		int yearInt = int.Parse( yearInput.text );
		yearInt++;
		yearInput.text = yearInt.ToString();
		year = yearInt.ToString();
		RefreshCalendar();
	}

	public void SetSelectedYear( string year ) {
		int yearInt = int.Parse( year );
		yearInt++;
		yearInput.text = yearInt.ToString();
		year = yearInt.ToString();
		RefreshCalendar();
	}

	// Previous year button
	public void PrevYear () {
		int yearInt = int.Parse( yearInput.text );
		yearInt--;
		yearInput.text = yearInt.ToString();
		year = yearInt.ToString();
		RefreshCalendar();
	}

	// Next month button
	public void NextMonth () {
		dateTime = dateTime.AddMonths( 1 );
		string monthTmp = dateTime.ToString("MMMM");
		SetSelectedMonth( monthTmp );
		SelectMonth( monthTmp );
	}
	
	// Previous month button
	public void PrevMonth () {
		dateTime = dateTime.AddMonths( -1 );
		string monthTmp = dateTime.ToString("MMMM");
		SetSelectedMonth( monthTmp );
		SelectMonth( monthTmp );
	}

	public void NextMonthYear () {
		NextMonth();
		int currentMonth = int.Parse( dateTime.ToString("MM") );
		if( currentMonth == 1 ) {
			NextYear();
		}
	}

	public void PrevMonthYear () {
		PrevMonth();
		int currentMonth = int.Parse( dateTime.ToString("MM") );
		if( currentMonth == 12 ) {
			PrevYear();
		}
	}

	public void DateToday() {
		SetDateTimeNow();
		RefreshCalendar();
	}

	public string GetSelectedMonth() {
		return monthButton.transform.GetChild(0).GetComponent<Text>().text;
	}
	
	public void SetSelectedMonth( string monthName ) {
		monthButton.transform.GetChild(0).GetComponent<Text>().text = monthName;
	}
	
	// pass on gui object, for later callback and fill fields
	public void SetCallbackObject ( GameObject target ) {
		canvasCalendar.SetActive( true );
		this.targetObject = target;
		isSaveDate = true;
		UpdateCalendar();
	}

	public void SelectDayObject ( GameObject target ) {
		this.selectDayObject = target;

		if( closeCalendarAfterSelect ) {
			CloseCalendar();
		}
		else {
			canvasCalendar.SetActive( true );
			isSaveDate = true;
			UpdateCalendar();
		}
	}

	public void InputTextEndEdit() {
		Debug.Log ( "InputTextEndEdit: " + yearInput.text );
		if( yearInput.text.Trim().Length == 4 ) {
			yearInput.transform.GetChild(1).GetComponent<Text>().color = Color.black;
			SetSelectedYear( yearInput.text );
		}
		else {
			yearInput.transform.GetChild(1).GetComponent<Text>().color = Color.red;
		}
	}


//	// pass on gui object, for later callback and fill fields, plus save the date in PlayerPrefs
//	public void SetCallbackObjectSaveDate ( GameObject target, string saveName ) {
//		targetObject = target;
//		isSaveDate = true;
//		canvasCalendar.SetActive( true );
//		UpdateCalendar();
//	}

	// will only save the date in PlayerPrefs
	public void SetCallbackSaveDate () {
//		Debug.Log( "SetCallbackSaveDate" );
		targetObject = null;
		isSaveDate = true;
		canvasCalendar.SetActive( true );
		UpdateCalendar();
	}

	// Fill the callback targetObject 
	// US and other date formats
	public void SetDate ( GameObject target ) {

		day = target.transform.GetChild(0).GetComponent<Text>().text;
		month = GetSelectedMonth();;
		year = yearInput.text;

		if( targetObject != null ) {

			LabelDateController ldc = targetObject.GetComponent<LabelDateController>();
			if( ldc != null ) {
				addZero = ldc.addZero;
				dateDelim = ldc.dateDelim;
//				ldc.dateLabel.text = SaveDate ( ldc.saveName );
				SaveDate ( ldc.saveDateName ); // save always selection
				ldc.LoadFormat();
				ldc.SetFormat();
			}
			else {
				if( usFormat ) {
					targetObject.transform.GetChild(0).GetComponent<InputField>().text = month;
					targetObject.transform.GetChild(1).GetComponent<InputField>().text = day;
					targetObject.transform.GetChild(2).GetComponent<InputField>().text = year;
				}
				else {
					targetObject.transform.GetChild(0).GetComponent<InputField>().text = day;
					targetObject.transform.GetChild(1).GetComponent<InputField>().text = month;
					targetObject.transform.GetChild(2).GetComponent<InputField>().text = year;
				}
				SaveDate (); // save always selection
			}
		}
		else {
//			Debug.LogError( "targetObject not set earlier in function SetCallbackObject!" );
//			SaveDate ();
		}

		canvasCalendar.SetActive( false );
	}

	public int GetDaysInMonth( int year, int month ) {
		return DateTime.DaysInMonth( year, month );
	}

	public void SelectMonth ( string monthName ) {
		targetMonthPanel.SetActive( false );
		month = monthName;
		SetSelectedMonth( monthName );

		// refresh months
		SetCalendar ( false );
		RefreshCalendar();
	}

	public void SelectMonth ( GameObject target ) {
	    month = target.transform.GetChild(0).GetComponent<Text>().text;
		SelectMonth ( month );
	}

	public void RefreshCalendar() {
		ClearCalendar ();
		monthDigit = CalendarUtils.GetMonthDigit( month, true ); 
		daysInMonth = GetDaysInMonth( int.Parse( year ), int.Parse( monthDigit ) );
//		Debug.Log( "RefreshCalendar - monthDigit: " + monthDigit + ", year: " + year + " , daysInMonth: " + daysInMonth );
		UpdateCalendar();
	}

	public string SaveDate ( string saveName ) {
		string saveDate = "";

		day = CalendarUtils.GetDayDigit( day, true);
		monthDigit = CalendarUtils.GetMonthDigit( month, true);
		monthDigitSaved = monthDigit;
		yearSaved = year;

		// Standarized for DateTime
		saveDate = year + "-" + monthDigit +  "-" + day;

//		if( usFormat ) {
//			saveDate = monthDigit + dateDelim + day + dateDelim + year;
//		}
//		else {
//			saveDate = day + dateDelim + monthDigit + dateDelim + year;
//		}
		
		PlayerPrefs.SetString( saveName, saveDate );
//		Debug.Log( "SaveDate date saved: " + saveDate );
		return saveDate;
	}

	public string SaveDate () {
		return SaveDate ( saveNameDate );
	}

	public void LoadDate () {
		string savedDate = PlayerPrefs.GetString( saveNameDate );
		if( savedDate.Equals("") ) savedDate = SaveDate ();
//		Debug.Log( "LoadDate date saved: " + savedDate );

		string[] saveDateSplit = savedDate.Split( '-' );

		// Standarized for DateTime
		year = saveDateSplit[0];
		monthDigit = saveDateSplit[1];
		day = saveDateSplit[2];

//		if( usFormat ) {
//			monthDigit = saveDateSplit[0];
//			day = saveDateSplit[1];
//			year = saveDateSplit[2];
//		}
//		else {
//			day = saveDateSplit[0];
//			monthDigit = saveDateSplit[1];
//			year = saveDateSplit[2];
//		}

		dateTime = DateTime.Parse( year + "/" + monthDigit  + "/" + day ); 
		month = dateTime.ToString("MMMM");

		monthDigitSaved = monthDigit;
		yearSaved = year;

		daysInMonth = GetDaysInMonth( int.Parse( year ), int.Parse( monthDigit ) );
//		Debug.Log( "LoadDate - savedDate: " + savedDate + ", daysInMonth: " + daysInMonth );
	}

	public string GetDate () {
		return PlayerPrefs.GetString( saveNameDate );
	}

	public void PanelMonthToggle () {
		targetMonthPanel.SetActive (!targetMonthPanel.activeSelf);
	}

	public void ToggleActive () {
		canvasCalendar.SetActive (!gameObject.activeSelf);
	}

	// Update is called once per frame
	void Update () {

	}
}
