/**
 * Hexa Snap
 * © Aurélien Lubecki 2019
 * All Rights Reserved
 */
 
using System.Collections.Generic;

public abstract class BaseNode : BaseModel {

    private static BaseNodeListener to(BaseModelListener listener) {
        return (BaseNodeListener) listener;
    }


    public readonly Graph graph;
    public readonly string tag;
    public readonly float linePercentageLevel;
    public readonly bool lineMustStartVerticalFirst;

    public BaseNodeSelectListener nodeSelectListener;

    public bool isReachable { get; private set; }


    public BaseNode(Graph graph, string tag) : this(graph, tag, 1, false) {
    }

    public BaseNode(Graph graph, string tag, float linePercentageLevel, bool lineMustStartVerticalFirst) {

        this.graph = graph;
        this.tag = tag;
        this.linePercentageLevel = linePercentageLevel;
        this.lineMustStartVerticalFirst = lineMustStartVerticalFirst;
    }

    public override int GetHashCode() {
        return 1877310944 + EqualityComparer<string>.Default.GetHashCode(tag);
    }

    public override bool Equals(object obj) {
        var node = obj as BaseNode;
        return node != null &&
               tag == node.tag;
    }

    public void onNodeSelect() {

        if (nodeSelectListener != null) {
            nodeSelectListener.onNodeSelect(this);
        }
        
    }

    public void setReachable(bool isReachable) {

        if (isReachable == this.isReachable) {
            return;
        }

        this.isReachable = isReachable;

        notifyListeners(listener => {
            to(listener).onNodeReachableChange(this);
        });

    }

}

public interface BaseNodeSelectListener {

    void onNodeSelect(BaseNode node);

}
