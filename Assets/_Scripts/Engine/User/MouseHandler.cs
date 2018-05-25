namespace Adventure.Engine.User
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    using Adventure.Engine.Navigation;
    using Adventure.Game;

    public class MouseHandler : MonoBehaviour
    {
        [SerializeField] LayerMask navGridLayerMask;

        public void ClickOnNavGrid(iControllable character)
        {
            if(Input.GetButtonDown("Click"))
            {
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);

                RaycastHit hit;
                if(Physics.Raycast(mouseRay, out hit, 500f, navGridLayerMask))
                {
                    if(hit.collider.tag == "NavGrid")
                    {
                        NavGrid navgrid = hit.collider.GetComponent<NavGrid>();
                        Vector2Int nodePosition = navgrid.WorldPointToNode(hit.point);
                        
                        if(nodePosition != NavGrid.NO_NODE)
                        {
                            character.MoveTo(nodePosition, navgrid);
                        }
                    }
                }
            }
        }

    }
}