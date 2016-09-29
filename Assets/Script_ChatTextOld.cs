using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

public class Script_ChatTextOld : MonoBehaviour {

	bool refresh;
	Queue<string> chatStrings;
	Queue<int> linesQueue;
	int lines;
	int linesTotal;
	StringBuilder oneLineBuilder;
	int maxNumberOfLines;
	int charactersPerLineCounter;
	int charactersPerLineLimit;
	int oneLineBuilderStartLength;

	// Use this for initialization
	void Start () {
		oneLineBuilderStartLength = 0;
		charactersPerLineLimit = 28;
		refresh = false;
		chatStrings = new Queue<string> ();
		linesQueue = new Queue<int> ();
		oneLineBuilder = new StringBuilder ();
		maxNumberOfLines = 19;
	}

	// Update is called once per frame
	void Update () {
		if (refresh) {
			GetComponent<Text> ().text = QueueOfStringsToString (chatStrings);
			refresh = false;
		}
	}

	string QueueOfStringsToString (Queue<string> queueOfStrings) {
		StringBuilder multiLineBuilder = new StringBuilder ();
		Queue<string> queueOfStringsClone = new Queue<string> (queueOfStrings);
		while (queueOfStringsClone.Count > 0) {
			multiLineBuilder.AppendLine (queueOfStringsClone.Dequeue ());
		}
		return multiLineBuilder.ToString ();
	}

	void AddNewLineToContainer (string newLine) {
		chatStrings.Enqueue (newLine);
		linesQueue.Enqueue (lines);
		linesTotal += lines;
		while (linesTotal > maxNumberOfLines) {
			chatStrings.Dequeue ();
			linesTotal -= linesQueue.Dequeue ();
		}
	}

	public void StartNewLine (string color) {
		lines = 1;
		oneLineBuilder = new StringBuilder ();
		oneLineBuilder.Append (color);
		oneLineBuilderStartLength += color.Length;
	}

	public void EndLine () {
		oneLineBuilderStartLength = 0;
		oneLineBuilder.Append ("</color>");
		charactersPerLineCounter = 0;
		AddNewLineToContainer (oneLineBuilder.ToString ());
		refresh = true;
	}

	public void AddText (string text) {
		while (charactersPerLineCounter + text.Length + 1 > charactersPerLineLimit) {
			int cutPosition = FindStringCutPosition (text, charactersPerLineCounter, charactersPerLineLimit);
			if (oneLineBuilder.Length == oneLineBuilderStartLength) {
				text = char.ToUpper (text [0]) + text.Substring (1);
				oneLineBuilder.Append (text.Substring (0, cutPosition));
			} else {
				oneLineBuilder.Append (" " + text.Substring (0, cutPosition));
			}
			oneLineBuilder.Append ("\n");
			lines++;
			charactersPerLineCounter = 0;
			text = text.Substring (cutPosition);
		}
		if (charactersPerLineCounter == 0) {
			if (oneLineBuilder.Length == oneLineBuilderStartLength) {
				text = char.ToUpper (text [0]) + text.Substring (1);
			}
			oneLineBuilder.Append (text);
			charactersPerLineCounter += text.Length;
		} else {
			oneLineBuilder.Append (" " + text);
			charactersPerLineCounter += text.Length + 1;
		}
	}

	public void AddComma () {
		oneLineBuilder.Append (",");
		charactersPerLineCounter++;
	}

	public void AddDot () {
		oneLineBuilder.Append (".");
		charactersPerLineCounter++;
	}

	int FindStringCutPosition (string s, int charPerLineCounter, int charLimit) {
		int lookPosition = (charLimit - charPerLineCounter) - 1;
		for (int i = lookPosition; i > 0; i--) {
			if (s [i] == ' ') {
				return i + 1;
			}
		}
		return 0;
	}

	public void EnableBold () {
		string bold = "<b>";
		oneLineBuilder.Append (bold);
		oneLineBuilderStartLength += bold.Length;
	}

	public void DisableBold () {
		oneLineBuilder.Append ("</b>");
	}
}
