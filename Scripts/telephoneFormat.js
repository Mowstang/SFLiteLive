/*
<TR>
	<TD WIDTH="129">
		<P ALIGN="right">Social Security #<BR><I><FONT SIZE="1">(Send a custom format)</FONT></I></P>
	</TD>
	<TD>
		<P><INPUT TYPE="text" NAME="first_name" SIZE="25" ONKEYPRESS="FormatNumber(this, '###-##-#### ');"> 
		ONKEYPRESS call: FormatNum(this, &quot;###-##-#### 
		&quot;)</P>
	</TD>
</TR>
<TR>
	<TD WIDTH="129">
		<P ALIGN="right">Phone<BR><I><FONT SIZE="1">(Use both long and short formats)</FONT></I></P>
	</TD>
	<TD>
		<P>
		<INPUT TYPE="text" NAME="phone" SIZE="25" ONKEYPRESS="FormatNumber(this, '(###) ###-#### ', '###-#### ');"> 
		ONKEYPRESS call: FormatNum(this, &quot;(###) ###-#### &quot;, &quot;###-#### &quot;)</P>
	</TD>
</TR>
*/

//-----------------------------------------------------------------------------------------------------------
//---------------------Number Formatter by ProWebMasters.net----------------------------------------
//---------------------  You are free to use this in your   ----------------------------------------
//---------------------  scripts, but you must leave this   ----------------------------------------
//---------------------   Copyright notice intact. Enjoy!   ----------------------------------------
//-----------------------------------------------------------------------------------------------------------
//--------------------- You can call this function with only----------------------------------------
//---------------------   the 'num' object (reference your  ----------------------------------------
//---------------------   field you want to format). If no  ----------------------------------------
//--------------------formats are sent, it will use default----------------------------------------
//---------------------             formats.                ----------------------------------------
//-----------------------------------------------------------------------------------------------------------

var globalCultureName = "en";           // default global culture to "en"glish in case it's not set

function FormatNumber(num, format, shortformat)
{
	if(format==null)
	{
		// Choose the default format you prefer for the number. 
		//format = "#-(###) ###-#### ";		// Telephone w/ LD Prefix and Area Code
		//format = "###-###-####";			// Telephone w/ Area Code (dash seperated)
		//format = "###-##-####";			//Social Security Number
		
		format = "(###) ###-#### ";			// Telephone w/ Area Code
	}					
//---------------------------------------------------------------
//---------------------------------------------------------------
	if(shortformat==null)
	{
		// Choose the short format (without area code) you prefer. 
		//If you do not want multiple formats, leave it as "".

		//var shortformat = "###-#### ";
		var shortformat = "";
	}
	
//----------------------------------------------------------------------------------------------------------
//----------------------- This code can be used to format any number. ---------------------------------
//----------------------- Simply change the format to a number format ---------------------------------
//----------------------- you prefer. It will ignore all characters  ----------------------------------
//----------------------- except the #, where it will replace with  -----------------------------------
//----------------------- user input. -----------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------

	var validchars = "0123456789";
	var tempstring = "";
	var returnstring = "";
	var extension = "";
	var tempstringpointer = 0;
	var returnstringpointer = 0;
	count = 0;

	// Get the length so we can go through and remove all non-numeric characters
	var length = num.value.length;
		

	// We are only concerned with the format of the phone number - extensions can be left alone.
	if (length > format.length)
	{
		length = format.length;
	};
	
	// scroll through what the user has typed
	for (var x=0; x<length; x++)
	{
		if (validchars.indexOf(num.value.charAt(x))!=-1)
		{
		tempstring = tempstring + num.value.charAt(x);
		};
	};
	// We should now have just the #'s - extract the extension if needed
	if (num.value.length > format.length)
	{
		length = format.length;
		extension = num.value.substr(format.length, (num.value.length-format.length));
	};
	
	// if we have fewer characters than our short format, we'll default to the short version.
	for (x=0; x<shortformat.length;x++)
	{
		if (shortformat.substr(x, 1)=="#")
		{
			count++;
		};
	}
	if (tempstring.length <= count)
	{
		format = shortformat;
	};

	
	//Loop through the format string and insert the numbers where we find a # sign
	for (x=0; x<format.length;x++)
	{
		if (tempstringpointer <= tempstring.length)
		{
			if (format.substr(x, 1)=="#")
			{
				returnstring = returnstring + tempstring.substr(tempstringpointer, 1);
				tempstringpointer++;
			}else{
				returnstring = returnstring + format.substr(x, 1);
			}
		}
		
	}

	// We have gone through the entire format, let's add the extension back on.
		returnstring = returnstring + extension;
	
	//we're done - let's return our value to the field.
	num.value = returnstring;
}

function format(nbr, decimals) {
    var neg = false;
    var chk = nbr;
    chk = chk.replace(",", "");
    chk = chk.replace(".", "");
    if ((chk * 1) < 0.0) {
        neg = true;
    }
    nbr = nbr.replace("-", "");
    var sep = ".";
    var ths = ",";
    if (globalCultureName != "en") {
        sep = ",";
        ths = " ";
        nbr = nbr.replace(/[^0-9-,]/g, "");
    }
    else {
        nbr = nbr.replace(/[^0-9-.]/g, "");
    }
    decimals = decimals || 2;                       // Default to 2 decimals

    var num2 = nbr.toString().split(sep);
    var thousands = num2[0].split('').reverse().join('').match(/.{1,3}/g).join(ths);
    var decimals = (num2[1]) ? sep + num2[1] : '';

    var fmt = thousands.split('').reverse().join('') + decimals;
    if (neg) fmt = "-" + fmt;
    return fmt;
}

//************************************************************************
// Round the Value
//************************************************************************
function round_decimals(original_number, decimals) {
    if (globalCultureName != "en") {
        original_number.value = original_number.value.replace(/[^0-9-,]/g, "");
        original_number.value = original_number.value.replace(",", ".");
    }
    else {
        original_number.value = original_number.value.replace(/[^0-9-.]/g, "");
    }
    if (IsNumeric(original_number.value)) {
        var result1 = original_number.value * Math.pow(10, decimals)
		var result2 = Math.round(result1)
		var result3 = result2 / Math.pow(10, decimals)
		var result4 = pad_with_zeros(result3, decimals)
		if (globalCultureName != "en") {
		    result4 = result4.replace(".", ",")
		}
		original_number.value = result4
    }
    else {
        alert('Please enter numeric values only.');
		original_number.value = "";
	    original_number.select();
		original_number.focus();				
	}
}
function round_decimals_span(original_number, decimals) {
    if (globalCultureName != "en") {
        original_number.innerHTML = original_number.innerHTML.replace(/[^0-9-,]/g, "");
        original_number.value = original_number.value.replace(",", ".");
    }
    else {
        original_number.innerHTML = original_number.innerHTML.replace(/[^0-9-.]/g, "");
    }
    if (IsNumeric(original_number.innerHTML))
	{
		var result1 = original_number.innerHTML * Math.pow(10, decimals)
		var result2 = Math.round(result1)
		var result3 = result2 / Math.pow(10, decimals)
		var result4 = pad_with_zeros(result3, decimals)
		if (globalCultureName != "en") {
		    result4 = result4.replace(".", ",")
		}
		original_number.innerHTML = result4
    }
    else {
	    alert('Please enter numeric values only.');
	    original_number.select();
		original_number.focus();
	}
}

function round_nonnegative_decimals(original_number, decimals)
{
    var v = false;
    if (globalCultureName != "en") {
        original_number.value = original_number.value.replace(/[^0-9-,]/g, "");
        original_number.value = original_number.value.replace(",", ".");
    }
    else {
        original_number.value = original_number.value.replace(/[^0-9-.]/g, "");
    }
    if (IsNumeric(original_number.value))
	{
	    if (parseFloat(original_number.value) >= 0)
	    {
			var result1 = original_number.value * Math.pow(10, decimals)
			var result2 = Math.round(result1)
			var result3 = result2 / Math.pow(10, decimals)
			var result4 = pad_with_zeros(result3, decimals)
			if (globalCultureName != "en") {
			    result4 = result4.replace(".", ",")
			}
			original_number.value = result4
			v = true;
	    }
	}
	if (v == false)
	{
	    alert('Negative value is not allowed.');
	    original_number.select();
	    original_number.focus();	    	
	}
	return v;

}

function round_positive_decimals(original_number, decimals)
{
    if (globalCultureName != "en") {
        original_number.value = original_number.value.replace(/[^0-9-,]/g, "");
        original_number.value = original_number.value.replace(",", ".");
    }
    else {
        original_number.value = original_number.value.replace(/[^0-9-.]/g, "");
    }
	if(IsNumeric(original_number.value))
	{
	    if (parseFloat(original_number.value) > 0)
	    {
	        var result1 = original_number.value * Math.pow(10, decimals)
	        var result2 = Math.round(result1)
	        var result3 = result2 / Math.pow(10, decimals)
	        var result4 = pad_with_zeros(result3, decimals)
	        if (globalCultureName != "en") {
	            result4 = result4.replace(".", ",")
	        }
	        original_number.value = result4
	        v = true;
	    }
	}
	if (v == false)
	{
	    original_number.select();
	    original_number.focus();	
	}
}

function pad_with_zeros(rounded_value, decimal_places) {
    var sep = ".";
    //  incoming rounded_value must have no thousands separator and
    //  the decimal seperator must be normalized to "."
    //

    var value_string = rounded_value.toString()         // Convert the number to a string
    var decimal_location = value_string.indexOf(sep)    // Locate the decimal separator

    // Is there a decimal point?
    if (decimal_location == -1) {                       // If no, then all decimal places will be padded with 0s
        decimal_part_length = 0
        value_string += decimal_places > 0 ? sep : ""   // tack on a decimal separator if required
    }
    else                                                // only the extra decimal places will be padded with 0s 
    {   
        decimal_part_length = value_string.length - decimal_location - 1
    }

    var pad_total = decimal_places - decimal_part_length // how many decimal places need to be padded with 0s
    if (pad_total > 0) {
        for (var counter = 1; counter <= pad_total; counter++)
            value_string += "0"
    }
    return value_string
}

function IsNumber(sText)
{
   var ValidChars = "0123456789 ";
   var IsNumber=true;
   var Char;

 
   for (i = 0; i < sText.length && IsNumber == true; i++) 
      { 
      Char = sText.charAt(i); 
      if (ValidChars.indexOf(Char) == -1) 
         {
         IsNumber = false;
         }
      }
   return IsNumber;
   
}

function IsNumeric(sText)
{
   var ValidChars = "-0123456789.";
   var IsNumber = true;
   var Char;

 
   for (i = 0; i < sText.length && IsNumber == true; i++) 
      { 
      Char = sText.charAt(i); 
      if (ValidChars.indexOf(Char) == -1) 
         {
         IsNumber = false;
         }
      }
   return IsNumber;
   
}


/*
	<input type="text" name="RedemptionAmount" value="550000" tabindex="1" onBlur="getNewAmount()">
*/
function checkNumeric(s)
{
var valid=true;
var field =leadingTrim(trailingTrim(s)).value;
if (field !="")
	{ 
	var sv=0;
		sv=parseFloat(field);
		if (sv != field) 
		{
			valid=false;
			s.select();
			alert("Invalid Number. Use numeric values.");
			s.focus();							
		}
		else
		{
			valid=true;		
		}
	}	
}

function toDigits(str) {
var i=0, nString="", digit, DotNo=0, olsValue=str.value;
	while (i<str.value.length) {
		digit=str.value.charAt(i);
		if (digit==".") DotNo=DotNo+1; 
		if(DotNo>1) {
        		str.select();
			alert("You can't have two '.' in the currency.")
	        	str.focus();
        		return(false); 
		}
		if (parseFloat(digit) || digit == "." || digit == "0") { 
			nString += digit
		}
		else {
	      		if (digit != " " && digit != "$" && digit != ","){ 
	        		str.select();
				alert("Please enter a non-zero numeric value.")
	        		str.focus();
        			return(oldValue); 
			}
		}

		i=i+1;
	}
	return(nString);	
}

function getNewAmount(control) 
{
	var Currency, amount, NumString;
	NumString=toDigits(control);
	control.value=toCurrency3(NumString);
}


function toCurrency3(num)
{
	var i, tCurrency, inx
	i=num.toString().indexOf(".");
	if (i==-1) i=num.toString().length;
	tCurrency= String(num).substr(0,i%3);
	if (i%3!=0 && i>3) 	tCurrency +=',';
	for (inx=0; inx<parseInt(i/3); inx++) 
	{
		tCurrency += String(num).substr(i%3+inx*3,3);
		if (inx+1<parseInt(i/3)) tCurrency += ',';
	}	
	
	tCurrency += String(num).substr(i,String(num).length-i);
	inx=tCurrency.indexOf(".");
	i=tCurrency.length;
	if ((i-inx) > 3 && inx>0) tCurrency=tCurrency.substr(0,inx+3);
	if (tCurrency.indexOf(".")==-1) tCurrency=tCurrency + ".00";
	if (i-tCurrency.indexOf(".")==1) tCurrency=tCurrency + "00";
	if (i-tCurrency.indexOf(".")==2) tCurrency=tCurrency + "0";
	return (tCurrency);
}

function CheckTextAreaLength(txt, length, fieldName)
{
    if (txt.value.length > length)
    {
       alert('You have exceeded the maximum for ' + fieldName);
       txt.focus();     
    }
}