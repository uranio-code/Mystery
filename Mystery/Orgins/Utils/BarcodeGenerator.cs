
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Linq;
using System.Drawing;
using Mystery.Properties;
using Mystery.Register;
using Mystery.Content;

[GlobalAvalibleObjectImplementation(singleton = true,implementation_of = typeof(IUidProvider))]
//public class BarcodeGenerator:IUidProvider
//{

//	private int length = 9;
//	private int sublength = 3;
//	private char separator = '-';
//	private int min_avaliability = 100;

//    private int request_avaliability;
    

//    private string generateUid()
//	{
//		string @base = string.Concat((from x  in Guid.NewGuid().ToString() where char.IsLetterOrDigit(x) select x)).Substring(0, length);
//		string result = string.Empty;
//		for (int i = 1; i <= @base.Length; i++) {
//			result += @base[i - 1];
//			if (i % sublength == 0 && i < @base.Length) {
//				result += separator;
//			}
//		}
//		return result;
//	}

//	private void refillIfNecessary()
//	{
//		IConnection conn = this.getGlobalObject<IConnection>();
//		string query = "select top " + min_avaliability + " count(1) from UidStore where content_guid is null and booking_request is null";
//		int available = (int)conn.executeScalar(query);
//		string insert_query = "INSERT INTO UidStore (uid) VALUES (@code)";
//		SqlParameter code_para = new SqlParameter("code", "hello");
//		while (available < min_avaliability) {
//			for (int i = 0; i <= request_avaliability; i++) {
//				//we do not care if it crashes
//				try {
//					code_para.Value = generateUid();
//					conn.executeNonQuery(insert_query, code_para);
//					Console.WriteLine(code_para.Value);
//				} catch {
//					//it mean same code it has been generated twice
//				}
//			}
//			available = (int)conn.executeScalar(query);
//		}
//	}

//	public BarcodeGenerator()
//	{
//        request_avaliability = min_avaliability * 10;
//        IConnection conn = this.getGlobalObject<IConnection>();
//		if (conn.executeQuery("select * from sys.tables where name='UidStore'").Rows.Count == 0) {
//			conn.executeNonQuery(Resources.TableBarcodeGenerationQuery);
//		}

//		//checking available
//		refillIfNecessary();

//	}

//	string IUidProvider.getUid()
//	{
//		string request = Guid.NewGuid().ToString();
//		string update_query = "update [UidStore] set booking_request=@request where id in (select top 1 id from  [UidStore] where content_guid is null and booking_request is null)";
//		SqlParameter request_para = new SqlParameter("request", request);
//		string select_query = "select top 1 uid from [UidStore] where booking_request=@request";
//		IConnection conn = this.getGlobalObject<IConnection>();
//		do {
//			conn.executeNonQuery(update_query, request_para);
//			DataTable dt = conn.executeQuery(select_query, request_para);
//			if (dt.Rows.Count == 0)
//				continue;
//			return (string)dt.Rows[0]["uid"];
//		} while (true);
//	}

//	void IUidProvider.registerUid(string uid, Guid guid)
//	{
//		string query = "update [UidStore] set content_guid=@guid, booking_request=NULL where uid=@uid";
//		SqlParameter contentID = new SqlParameter("guid", guid.ToString());
//		SqlParameter code_para = new SqlParameter("uid", uid);
//		IConnection conn = this.getGlobalObject<IConnection>();
//		conn.executeNonQuery(query, contentID, code_para);
//	}


//}

public class Code128ARendering
{

	#region "Code Pattern"
	/// <summary>
	/// Contains Pattern for 0,1,2,3,4......till 106
	/// in principle these rows should each have 6 elements
	/// however, the last one -- STOP -- has 7. The cost of the
	/// extra integers is trivial, and this lets the code flow
	/// much more elegantly
	/// </summary>
	/// <remarks></remarks>
	private static readonly int[,] cPatterns = {
		{
			2,
			1,
			2,
			2,
			2,
			2,
			0,
			0
		},
		{
			2,
			2,
			2,
			1,
			2,
			2,
			0,
			0
		},
		{
			2,
			2,
			2,
			2,
			2,
			1,
			0,
			0
		},
		{
			1,
			2,
			1,
			2,
			2,
			3,
			0,
			0
		},
		{
			1,
			2,
			1,
			3,
			2,
			2,
			0,
			0
		},
		{
			1,
			3,
			1,
			2,
			2,
			2,
			0,
			0
		},
		{
			1,
			2,
			2,
			2,
			1,
			3,
			0,
			0
		},
		{
			1,
			2,
			2,
			3,
			1,
			2,
			0,
			0
		},
		{
			1,
			3,
			2,
			2,
			1,
			2,
			0,
			0
		},
		{
			2,
			2,
			1,
			2,
			1,
			3,
			0,
			0
		},
		{
			2,
			2,
			1,
			3,
			1,
			2,
			0,
			0
		},
		{
			2,
			3,
			1,
			2,
			1,
			2,
			0,
			0
		},
		{
			1,
			1,
			2,
			2,
			3,
			2,
			0,
			0
		},
		{
			1,
			2,
			2,
			1,
			3,
			2,
			0,
			0
		},
		{
			1,
			2,
			2,
			2,
			3,
			1,
			0,
			0
		},
		{
			1,
			1,
			3,
			2,
			2,
			2,
			0,
			0
		},
		{
			1,
			2,
			3,
			1,
			2,
			2,
			0,
			0
		},
		{
			1,
			2,
			3,
			2,
			2,
			1,
			0,
			0
		},
		{
			2,
			2,
			3,
			2,
			1,
			1,
			0,
			0
		},
		{
			2,
			2,
			1,
			1,
			3,
			2,
			0,
			0
		},
		{
			2,
			2,
			1,
			2,
			3,
			1,
			0,
			0
		},
		{
			2,
			1,
			3,
			2,
			1,
			2,
			0,
			0
		},
		{
			2,
			2,
			3,
			1,
			1,
			2,
			0,
			0
		},
		{
			3,
			1,
			2,
			1,
			3,
			1,
			0,
			0
		},
		{
			3,
			1,
			1,
			2,
			2,
			2,
			0,
			0
		},
		{
			3,
			2,
			1,
			1,
			2,
			2,
			0,
			0
		},
		{
			3,
			2,
			1,
			2,
			2,
			1,
			0,
			0
		},
		{
			3,
			1,
			2,
			2,
			1,
			2,
			0,
			0
		},
		{
			3,
			2,
			2,
			1,
			1,
			2,
			0,
			0
		},
		{
			3,
			2,
			2,
			2,
			1,
			1,
			0,
			0
		},
		{
			2,
			1,
			2,
			1,
			2,
			3,
			0,
			0
		},
		{
			2,
			1,
			2,
			3,
			2,
			1,
			0,
			0
		},
		{
			2,
			3,
			2,
			1,
			2,
			1,
			0,
			0
		},
		{
			1,
			1,
			1,
			3,
			2,
			3,
			0,
			0
		},
		{
			1,
			3,
			1,
			1,
			2,
			3,
			0,
			0
		},
		{
			1,
			3,
			1,
			3,
			2,
			1,
			0,
			0
		},
		{
			1,
			1,
			2,
			3,
			1,
			3,
			0,
			0
		},
		{
			1,
			3,
			2,
			1,
			1,
			3,
			0,
			0
		},
		{
			1,
			3,
			2,
			3,
			1,
			1,
			0,
			0
		},
		{
			2,
			1,
			1,
			3,
			1,
			3,
			0,
			0
		},
		{
			2,
			3,
			1,
			1,
			1,
			3,
			0,
			0
		},
		{
			2,
			3,
			1,
			3,
			1,
			1,
			0,
			0
		},
		{
			1,
			1,
			2,
			1,
			3,
			3,
			0,
			0
		},
		{
			1,
			1,
			2,
			3,
			3,
			1,
			0,
			0
		},
		{
			1,
			3,
			2,
			1,
			3,
			1,
			0,
			0
		},
		{
			1,
			1,
			3,
			1,
			2,
			3,
			0,
			0
		},
		{
			1,
			1,
			3,
			3,
			2,
			1,
			0,
			0
		},
		{
			1,
			3,
			3,
			1,
			2,
			1,
			0,
			0
		},
		{
			3,
			1,
			3,
			1,
			2,
			1,
			0,
			0
		},
		{
			2,
			1,
			1,
			3,
			3,
			1,
			0,
			0
		},
		{
			2,
			3,
			1,
			1,
			3,
			1,
			0,
			0
		},
		{
			2,
			1,
			3,
			1,
			1,
			3,
			0,
			0
		},
		{
			2,
			1,
			3,
			3,
			1,
			1,
			0,
			0
		},
		{
			2,
			1,
			3,
			1,
			3,
			1,
			0,
			0
		},
		{
			3,
			1,
			1,
			1,
			2,
			3,
			0,
			0
		},
		{
			3,
			1,
			1,
			3,
			2,
			1,
			0,
			0
		},
		{
			3,
			3,
			1,
			1,
			2,
			1,
			0,
			0
		},
		{
			3,
			1,
			2,
			1,
			1,
			3,
			0,
			0
		},
		{
			3,
			1,
			2,
			3,
			1,
			1,
			0,
			0
		},
		{
			3,
			3,
			2,
			1,
			1,
			1,
			0,
			0
		},
		{
			3,
			1,
			4,
			1,
			1,
			1,
			0,
			0
		},
		{
			2,
			2,
			1,
			4,
			1,
			1,
			0,
			0
		},
		{
			4,
			3,
			1,
			1,
			1,
			1,
			0,
			0
		},
		{
			1,
			1,
			1,
			2,
			2,
			4,
			0,
			0
		},
		{
			1,
			1,
			1,
			4,
			2,
			2,
			0,
			0
		},
		{
			1,
			2,
			1,
			1,
			2,
			4,
			0,
			0
		},
		{
			1,
			2,
			1,
			4,
			2,
			1,
			0,
			0
		},
		{
			1,
			4,
			1,
			1,
			2,
			2,
			0,
			0
		},
		{
			1,
			4,
			1,
			2,
			2,
			1,
			0,
			0
		},
		{
			1,
			1,
			2,
			2,
			1,
			4,
			0,
			0
		},
		{
			1,
			1,
			2,
			4,
			1,
			2,
			0,
			0
		},
		{
			1,
			2,
			2,
			1,
			1,
			4,
			0,
			0
		},
		{
			1,
			2,
			2,
			4,
			1,
			1,
			0,
			0
		},
		{
			1,
			4,
			2,
			1,
			1,
			2,
			0,
			0
		},
		{
			1,
			4,
			2,
			2,
			1,
			1,
			0,
			0
		},
		{
			2,
			4,
			1,
			2,
			1,
			1,
			0,
			0
		},
		{
			2,
			2,
			1,
			1,
			1,
			4,
			0,
			0
		},
		{
			4,
			1,
			3,
			1,
			1,
			1,
			0,
			0
		},
		{
			2,
			4,
			1,
			1,
			1,
			2,
			0,
			0
		},
		{
			1,
			3,
			4,
			1,
			1,
			1,
			0,
			0
		},
		{
			1,
			1,
			1,
			2,
			4,
			2,
			0,
			0
		},
		{
			1,
			2,
			1,
			1,
			4,
			2,
			0,
			0
		},
		{
			1,
			2,
			1,
			2,
			4,
			1,
			0,
			0
		},
		{
			1,
			1,
			4,
			2,
			1,
			2,
			0,
			0
		},
		{
			1,
			2,
			4,
			1,
			1,
			2,
			0,
			0
		},
		{
			1,
			2,
			4,
			2,
			1,
			1,
			0,
			0
		},
		{
			4,
			1,
			1,
			2,
			1,
			2,
			0,
			0
		},
		{
			4,
			2,
			1,
			1,
			1,
			2,
			0,
			0
		},
		{
			4,
			2,
			1,
			2,
			1,
			1,
			0,
			0
		},
		{
			2,
			1,
			2,
			1,
			4,
			1,
			0,
			0
		},
		{
			2,
			1,
			4,
			1,
			2,
			1,
			0,
			0
		},
		{
			4,
			1,
			2,
			1,
			2,
			1,
			0,
			0
		},
		{
			1,
			1,
			1,
			1,
			4,
			3,
			0,
			0
		},
		{
			1,
			1,
			1,
			3,
			4,
			1,
			0,
			0
		},
		{
			1,
			3,
			1,
			1,
			4,
			1,
			0,
			0
		},
		{
			1,
			1,
			4,
			1,
			1,
			3,
			0,
			0
		},
		{
			1,
			1,
			4,
			3,
			1,
			1,
			0,
			0
		},
		{
			4,
			1,
			1,
			1,
			1,
			3,
			0,
			0
		},
		{
			4,
			1,
			1,
			3,
			1,
			1,
			0,
			0
		},
		{
			1,
			1,
			3,
			1,
			4,
			1,
			0,
			0
		},
		{
			1,
			1,
			4,
			1,
			3,
			1,
			0,
			0
		},
		{
			3,
			1,
			1,
			1,
			4,
			1,
			0,
			0
		},
		{
			4,
			1,
			1,
			1,
			3,
			1,
			0,
			0
		},
		{
			2,
			1,
			1,
			4,
			1,
			2,
			0,
			0
		},
		{
			2,
			1,
			1,
			2,
			1,
			4,
			0,
			0
		},
		{
			2,
			1,
			1,
			2,
			3,
			2,
			0,
			0
		},
		{
			2,
			3,
			3,
			1,
			1,
			1,
			2,
			0
		}

	};
	#endregion

	#region "Make Barcode"
	#region "Constant"
	private const int cQuietWidth = 5;
		#endregion
	private const int Height = 15;

	/// <summary>
	/// Make an image of a Code128A barcode for a given string
	/// </summary>
	/// <param name="InputData">Message to be encoded</param>
	/// <param name="BarWeight">Base thickness for bar width (1 or 2 works well)</param>
	/// <param name="BarHeight">Heigth of the bar</param>
	/// <param name="AddQuietZone">Add required horiz margins (use if output is tight)</param>
	/// <returns>An Image of the Code128 barcode representing the message</returns>
	public static Image MakeBarcodeImage(string InputData, int BarWeight, int BarHeight, bool AddQuietZone, string second_text = "")
	{

		// get the Code128 codes to represent the message
		Code128AContent content = new Code128AContent(InputData);

		//'if the ascii values are improper then return nothing..operation closed.
		if (content.Codes == null) {
			return null;
		}

		int[] codes = content.Codes;

		Font drawFont = new Font("Arial", 10);

		int width = 0;
		int height = 0;
		int x = 0;
		int y = 0;
		x = 0;
		y = 5;
        int[] widths = {((codes.Length - 3) * 11 + 35) * BarWeight,
            InputData.Length* (int)drawFont.Size,
            second_text.Length* (int)drawFont.Size
        };
        width = widths.Max();
		width = ((codes.Length - 3) * 11 + 35) * BarWeight;
		//height = Convert.ToInt32( System.Math.Ceiling( Convert.ToSingle(width) * .15F) );
		height = BarHeight;

		if (AddQuietZone) {
			// on both sides
			width += 2 * cQuietWidth * BarWeight;
		}

		int img_height = height + 25;

		if (!string.IsNullOrEmpty(second_text))
			img_height += (int)drawFont.Size;


		// get surface to draw on
		Image myimg = new System.Drawing.Bitmap(width, img_height);
		using (Graphics gr = Graphics.FromImage(myimg)) {




			// set to white so we don't have to fill the spaces with white
			gr.FillRectangle(System.Drawing.Brushes.Red, 0, 0, width, img_height);

			// skip quiet zone
			int cursor = AddQuietZone ? cQuietWidth * BarWeight : 0;

			for (int codeidx = 0; codeidx <= codes.Length - 1; codeidx++) {
				int code = codes[codeidx];

				// take the bars two at a time: a black and a white
				for (int bar = 0; bar <= 7; bar += 2) {
					int barwidth = cPatterns[code, bar] * BarWeight;
					int spcwidth = cPatterns[code, bar + 1] * BarWeight;

					// if width is zero, don't try to draw it
					if (barwidth > 0) {
						gr.FillRectangle(System.Drawing.Brushes.Black, cursor, y, barwidth, height);
					}

					// note that we never need to draw the space, since we
					// initialized the graphics to all white

					// advance cursor beyond this pair
					cursor += (barwidth + spcwidth);
				}
			}


			// set to white so we don't have to fill the spaces with white
			x = Convert.ToInt32((width / 2) - (((InputData.Length) * 10) / 2));
			y += (BarHeight);

			gr.DrawString(InputData, drawFont, Brushes.Black, x, y);

			y += ((int)drawFont.Size);

			if (!string.IsNullOrEmpty(second_text)) {
				x = 5;
				// CInt((width / 2) - (((second_text.Length) * 10) / 2))
				gr.DrawString(second_text, drawFont, Brushes.Black, x, y);
			}

		}

		return myimg;
	}
	#endregion

}

public enum CodeSet
{
	CodeA
}

/// <summary>
/// Represent the set of code values to be output into barcode form
/// </summary>
public class Code128AContent
{
	private int[] mCodeList;
	/// <summary>
	/// Create content based on a string of ASCII data
	/// </summary>
	/// <param name="AsciiData">the string that should be represented</param>
	public Code128AContent(string AsciiData)
	{
		if (checkValidAscii(AsciiData)) {
			mCodeList = StringToCode128A(AsciiData);
		}
	}

	/// <summary>
	/// Provides the Code128 code values representing the object's string
	/// </summary>
	public int[] Codes {
		get { return mCodeList; }
	}


	/// <summary>
	/// Check the validity of the input string by comparing the ascii value
	/// Code128A : ASCII characters 00 to 95 (0-9, A-Z and control codes) and special characters ..i.e (00 to 106)
	/// </summary>
	/// <param name="AsciiData">Input String</param>
	/// <returns>T/F</returns>
	/// <remarks></remarks>
	private bool checkValidAscii(string AsciiData)
	{
		// turn the string into ascii byte data
		byte[] asciiBytes = Encoding.ASCII.GetBytes(AsciiData);

		foreach (byte ascii in asciiBytes) {
			if (ascii > 106) {
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Transform the string into integers representing the Code128 codes
	/// necessary to represent it
	/// </summary>
	/// <param name="AsciiData">String to be encoded</param>
	/// <returns>Code128 representation</returns>
	private int[] StringToCode128A(string AsciiData)
	{
		// turn the string into ascii byte data
		byte[] asciiBytes = Encoding.ASCII.GetBytes(AsciiData);

		// decide which codeset to start with
		CodeSet currcs = CodeSet.CodeA;

		// set up the beginning of the barcode
		System.Collections.ArrayList codes = new System.Collections.ArrayList(asciiBytes.Length + 3);

		// assume no codeset changes, account for start, checksum, and stop
		codes.Add(Code128Code.StartCodeForCodeSet(currcs));

		// add the codes for each character in the string
		for (int i = 0; i <= asciiBytes.Length - 1; i++) {
			int thischar = asciiBytes[i];
			int nextchar = asciiBytes.Length > (i + 1) ? asciiBytes[i + 1] : -1;
			codes.AddRange(Code128Code.CodesForChar(thischar, nextchar, ref currcs));
		}

		// calculate the check digit
		int checksum = (int)(codes[0]);
		for (int cnt_i = 1; cnt_i <= codes.Count - 1; cnt_i++) {
			checksum += cnt_i * (int)(codes[cnt_i]);
		}

		codes.Add(checksum % 103);
		codes.Add(Code128Code.StopCode());

		int[] result = codes.ToArray(typeof(int)) as int[];
		return result;
	}
}

/// <summary>
/// Static tools for determining codes for individual characters in the content
/// </summary>
public class Code128Code
{
	#region "Constants"

	private const int cSHIFT = 98;
	private const int cCODEA = 101;

	private const int cCODEB = 100;
	private const int cSTARTA = 103;
	private const int cSTARTB = 104;
		#endregion
	private const int cSTOP = 106;

	/// <summary>
	/// Get the Code128 code value(s) to represent an ASCII character, with
	/// optional look-ahead for length optimization
	/// </summary>
	/// <param name="CharAscii">The ASCII value of the character to translate</param>
	/// <param name="LookAheadAscii">The next character in sequence (or -1 if none)</param>
	/// <param name="CurrCodeSet">The current codeset, that the returned codes need to follow;
	/// if the returned codes change that, then this value will be changed to reflect it</param>
	/// <returns>An array of integers representing the codes that need to be output to produce the
	/// given character</returns>
	public static int[] CodesForChar(int CharAscii, int LookAheadAscii, ref CodeSet CurrCodeSet)
	{
		int[] result = null;

		result = new int[1];
		result[0] = CodeValueForChar(CharAscii);

		return result;
	}

	/// <summary>
	/// Gets the integer code128 code value for a character (assuming the appropriate code set)
	/// </summary>
	/// <param name="CharAscii">character to convert</param>
	/// <returns>code128 symbol value for the character</returns>
	public static int CodeValueForChar(int CharAscii)
	{
		return (CharAscii >= 32) ? CharAscii - 32 : CharAscii + 64;
	}

	/// <summary>
	/// Return the appropriate START code depending on the codeset we want to be in
	/// </summary>
	/// <param name="cs">The codeset you want to start in</param>
	/// <returns>The code128 code to start a barcode in that codeset</returns>
	public static int StartCodeForCodeSet(CodeSet cs)
	{
		return cs == CodeSet.CodeA ? cSTARTA : cSTARTB;
	}

	/// <summary>
	/// Return the Code128 stop code
	/// </summary>
	/// <returns>the stop code</returns>
	public static int StopCode()
	{
		return cSTOP;
	}

	/// <summary>
	/// Indicates which code sets can represent a character -- CodeA, CodeB, or either
	/// </summary>
	public enum CodeSetAllowed
	{
		CodeA
	}

}