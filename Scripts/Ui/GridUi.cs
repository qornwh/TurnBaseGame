using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridUi : MonoBehaviour
{
    [SerializeField] public int count;
    [SerializeField] private int type;
    [SerializeField] private int spacingX;
    [SerializeField] private int spacingY;
    
    void Start()
    {
        var grid = gameObject.GetComponent<GridLayoutGroup>();
        var rect = gameObject.GetComponent<RectTransform>().rect;
        if (grid != null && count > 0)
        {
            grid.spacing = new Vector2(spacingX, spacingY);
            float width = rect.width - grid.padding.left - grid.padding.right;
            float height = rect.height - grid.padding.top - grid.padding.bottom;
            Vector2 cellSize = new Vector2(width, height);
            grid.constraintCount = count;
            if (type == 1)
            {
                // col
                grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                width -= (grid.spacing.x * (count - 1));
                width /= count;
                cellSize.x = width;
                cellSize.y = width;
            }
            else if (type == 2)
            {
                // row
                grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                height -= (grid.spacing.y * (count - 1));
                height /= count;
                cellSize.y = height;
            }
            grid.cellSize = cellSize;
        }
    }
}
