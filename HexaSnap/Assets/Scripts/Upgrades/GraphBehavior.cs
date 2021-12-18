/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 
using System.Collections.Generic;
using UnityEngine;

public class GraphBehavior : BaseModelBehavior {

    public Graph graph {
        get {
            return (Graph) model;
        }
    }
    

    private static GameObject prefabNodeZone;
    private static GameObject prefabNodeBonusType;


    protected override void onInit() {
        base.onInit();

        if (prefabNodeZone == null) {
            prefabNodeZone = GameHelper.Instance.loadPrefabAsset(Constants.PREFAB_NAME_NODE_ZONE);
            prefabNodeBonusType = GameHelper.Instance.loadPrefabAsset(Constants.PREFAB_NAME_NODE_BONUS_TYPE);
        }

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, -150);

        initNodesGameObjects(null, null);
    }

    protected override void onDeinit() {
        base.onDeinit();
        
        prefabNodeZone = null;
        prefabNodeBonusType = null;
        
        foreach (BaseNodeBehavior b in GetComponentsInChildren<BaseNodeBehavior>()) {
            //remove manually to put the lines in the pool
            b.setLinkLine(null);
        }

    }

    private void initNodesGameObjects(BaseNode parentNode, Transform parentTransform) {

        string parentTag = (parentNode == null) ? Graph.ROOT : parentNode.tag;

        List<BaseNode> children = graph.getChildrenNodes(parentTag);
        if (children == null) {
            return;
        }

        Vector3 parentPos = (parentTransform == null) ? Vector3.zero : parentTransform.localPosition;

        foreach (BaseNode n in children) {
            
            GameObject goNode;

            if (n is NodeZone) {
                //zone
                goNode = GameObject.Instantiate(prefabNodeZone, transform);
                
            } else {
                //bonusType
                goNode = GameObject.Instantiate(prefabNodeBonusType, transform);
                
                //move under the zone
                goNode.transform.SetAsFirstSibling();
            }

            goNode.name = n.tag;
            goNode.transform.localPosition = getNextNodePos(parentPos, parentNode, n);

            BaseNodeBehavior nodeBehavior = goNode.GetComponent<BaseNodeBehavior>();
            nodeBehavior.init(n);

            if (parentTransform != null) {

                Line line = new Line(
                    parentTransform.position,
                    nodeBehavior.transform.position,
                    SegmentThickness.THIN,
                    n.linePercentageLevel,
                    n.lineMustStartVerticalFirst
                );

                nodeBehavior.setLinkLine(line);
            }

            initNodesGameObjects(n, goNode.transform);
        }
        
    }

    private Vector3 getNextNodePos(Vector3 parentPos, BaseNode parentNode, BaseNode currentNode) {

        if (currentNode is NodeZone) {
            Vector2 pos = (currentNode as NodeZone).posInGraph;
            return new Vector3(pos.x, pos.y, 0);
        }

        if (parentNode is NodeZone) {
            //zone => bonus
            float x = parentPos.x;
            float y = parentPos.y - 200;

            NodeBonusType node = currentNode as NodeBonusType;
            if (node.bonusType.isMalus) {
                x += 80;
            } else {
                x -= 80;
            }

            return new Vector3(x, y, 0);
        }

        //bonus => bonus
        return new Vector3(parentPos.x, parentPos.y - 160, 0);
    }

}
