using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NavGrid))]
public class NavGridCI : Editor
{
    int MAX_HANDLES_PER_FRAME = 500;
    int MIN_SECTION_SIZE = 15;

    private NavGrid navGrid;
    private SubGrid primarySubGrid;

    SerializedProperty nodesProp, widthProp, heightProp, offsetProp;

    private void OnEnable()
    {
        navGrid = target as NavGrid;

        nodesProp = serializedObject.FindProperty("nodes");
        widthProp = serializedObject.FindProperty("width");
        heightProp = serializedObject.FindProperty("height");
        offsetProp = serializedObject.FindProperty("positionOffset");

        PrepareGridForDisplay(widthProp.intValue, heightProp.intValue, navGrid.transform.position + offsetProp.vector3Value);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (GUILayout.Button("Generate Nodes"))
        {
            navGrid.GenerateNodes();
        }

        EditorGUILayout.PropertyField(widthProp);
        EditorGUILayout.PropertyField(heightProp);

        EditorGUILayout.Space();

        EditorGUILayout.PropertyField(offsetProp);

        serializedObject.ApplyModifiedProperties();
        //base.OnInspectorGUI();
    }


    //Prepares the grid for display by dividing it into smaller sections as needed
    //This is needed as drawing too many Handles at once slows down the editor
    private void PrepareGridForDisplay(int width, int height, Vector3 position)
    {
        primarySubGrid = new SubGrid(width, height, position);

        DivideSubGrid(primarySubGrid);
    }

    //Recursively divides the provided subgrid until acceptable sizes are reached
    private void DivideSubGrid(SubGrid theGrid)
    {
        int width, height;
        width = theGrid.GetWidth();
        height = theGrid.GetHeight();

        //Determine how many sections we will be dividing into
        int numberOfSections = DetermineNumberOfSections((width * height) / (float)MAX_HANDLES_PER_FRAME);
        if(numberOfSections == 1)
        {
            return;
        }

        //If we only need 2 sections, divide the longest side
        if(numberOfSections == 2)
        {
            if(width > height)
            {

            }
            else
            {

            }
        }

        //Calculate how much to divide each side by
        else
        {
            int widthDivisions, heightDivisions;
            int divisor = Mathf.CeilToInt(Mathf.Sqrt(numberOfSections));       //All possible numberOfSections (except 2) are perfect squares

            //Do the shortest side first
            if (width < height)
            {
                widthDivisions = CalculateDivisions(width, divisor);

                //Calculate how many divisions are needed for the other side
                if (widthDivisions != divisor)
                {
                    divisor = Mathf.CeilToInt((numberOfSections / (float)widthDivisions));
                }
                heightDivisions = divisor;
            }
            else
            {
                heightDivisions = CalculateDivisions(height, divisor);

                //Calculate how many divisions are needed for the other side
                if (heightDivisions != divisor)
                {
                    divisor = Mathf.CeilToInt((numberOfSections / (float)heightDivisions));
                }
                widthDivisions = divisor;
            }



            



        }


        



        foreach(SubGrid grid in theGrid.GetSubGrids())
        {
            DivideSubGrid(grid);
        }
    }

    //Calculates how many times a side should be divided, ensuring the resulting slices are at least MIN_SECTION_SIZE
    private int CalculateDivisions(int sideLength, int divisor)
    {
        int divisions = 1;

        for (int i = divisor; i > 1; i--)
        {
            if ((sideLength / (float)i) >= MIN_SECTION_SIZE)
            {
                divisions = i;
                break;
            }
        }

        return divisions;
    }

    //Returns how many sections the grid should be divided into
    private int DetermineNumberOfSections(float targetAmount)
    {
        if (targetAmount <= 1f)
            return 1;

        if (targetAmount <= 2f)
            return 2;

        if (targetAmount <= 4)
            return 4;

        if (targetAmount <= 9f)
            return 9;

        if (targetAmount <= 16f)
            return 16;

        if (targetAmount <= 25f)
            return 25;

        return 36;
    }


    //Called per frame for the editor, not the game
    private void OnSceneGUI()
    {
        int width = widthProp.intValue;
        int height = heightProp.intValue;

        //DrawNodeControls(width, height, nodesProp, navGrid.transform.position + offsetProp.vector3Value);





    }





    




































    private void DrawNodeControls(int width, int height, SerializedProperty nodes, Vector3 startPosition)
    {
        float nodeSize = 0.65f;
        float controlSize = 0.1f;
        float controlOffset = 0.2f;

        if (width <= 0 || height <= 0 || nodes.arraySize != width * height)
        {
            return;
        }

        Color nodeColor = new Color(1, 0, 0.2f, 0.45f);
        Color controlColor = Color.cyan;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 position = startPosition + new Vector3(x, 0, y);


                //Draw node
                Handles.color = nodeColor;
                Handles.SphereHandleCap(-1, position, Quaternion.identity, nodeSize, EventType.Repaint);

                /*

                //Draw controls
                Handles.color = controlColor;
                    //Up
                if(Handles.Button(position + Vector3.forward * controlOffset, Quaternion.identity, controlSize, controlSize, Handles.SphereHandleCap))
                {

                }
                    //Down
                else if(Handles.Button(position + Vector3.forward * -controlOffset, Quaternion.identity, controlSize, controlSize, Handles.SphereHandleCap))
                {
                    
                }
                    //Right
                if (Handles.Button(position + Vector3.right * controlOffset, Quaternion.identity, controlSize, controlSize, Handles.SphereHandleCap))
                {

                }
                    //Left
                else if (Handles.Button(position + Vector3.right * -controlOffset, Quaternion.identity, controlSize, controlSize, Handles.SphereHandleCap))
                {

                }
                */
            }
        }

        //Handles.Label(navGrid.transform.position, "Nav Grid");


    }

    /**
     * A structure thats contains a piece of the navGrid
     * As well as possibly more SubGrids
     */
    struct SubGrid
    {
        int width, height;
        Vector3 position;
        List<SubGrid> subGrids;

        public SubGrid(int width, int height, Vector3 position)
        {
            this.width = width;
            this.height = height;
            this.position = position;

            subGrids = new List<SubGrid>();
        }

        public void AddSubGrid(SubGrid newGrid)
        {
            subGrids.Add(newGrid);
        }

        public List<SubGrid> GetSubGrids()
        {
            return subGrids;
        }


        public int GetWidth()
        {
            return width;
        }

        public int GetHeight()
        {
            return height;
        }

    }

}

