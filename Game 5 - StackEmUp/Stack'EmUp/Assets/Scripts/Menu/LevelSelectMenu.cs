using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectMenu : Menu
{

    //Number of level buttons to generate
    [SerializeField]
    private float levelCount = 1;

    [Header("Generation Info")]
    [SerializeField]
    private GameObject levelButtonPrefab;

    //Scroll Rect for level buttons to go in
    [SerializeField]
    private RectTransform contentArea;

    //Postion within content area that we should start from 
    [SerializeField]
    private Vector3 startPoint;

    //Rows, columns and spacing
    [Header("Display Info")]
    [SerializeField]
    private int levelsPerRow;
    [SerializeField]
    private float rowSpacing, columnsSpacing;

    //Scene that we shoud load when the "start button is pressed"
    private Scene nextScene;

    // Start is called before the first frame update
    void Start()
    {

        int currentRow = 0;
        int currentRowItem = 0; //Current Item in the row

        for(int i = 0; i < levelCount; ++i)
        {
            //Create button
            GameObject button = Instantiate(levelButtonPrefab);
            button.transform.SetParent(contentArea);
            button.transform.localScale = Vector3.one;

            //Calculate it's postion
            RectTransform buttonRect = button.GetComponent<RectTransform>();
            Vector3 positionInGrid = new Vector3(currentRowItem * rowSpacing, currentRow * -columnsSpacing, 0f);
            Vector3 createPos = startPoint + positionInGrid;
            buttonRect.localPosition = createPos;

            //Set Level ID of item
            LevelMenuItem levelData = button.GetComponent<LevelMenuItem>();
            if(levelData != null)
            {
                levelData.InitLevelButton(i + 1, this);
            }

            //Check if we shoud go to the next row
            currentRowItem++;
            if(currentRowItem % levelsPerRow == 0)
            {
                currentRow++;
                currentRowItem = 0;
            }
            
        }

        //Work out the required size of the content box so that the scroll rect
        //works properly
        float requiredContentSize = Mathf.Abs(startPoint.y) + ((currentRow + 1) * columnsSpacing);
        contentArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, requiredContentSize);

    }
}
