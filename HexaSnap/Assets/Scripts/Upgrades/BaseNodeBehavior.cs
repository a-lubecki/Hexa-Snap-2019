/**
* Hexa Snap
* © Aurélien Lubecki 2019
* All Rights Reserved
*/

using UnityEngine;

public abstract class BaseNodeBehavior : BaseModelBehavior, BaseNodeListener {

    public BaseNode node {
        get {
            return (BaseNode) model;
        }
    }


    private Line line;
    private LineDrawer lineDrawer;


    protected override void onInit() {
        base.onInit();

        updateLineColor();
    }

    protected override void onDeinit() {
        base.onDeinit();

        setLinkLine(null);
    }


    public void setLinkLine(Line newLine) {

        if (!isInit()) {
            return;
        }

        if (line != null) {

            GameHelper.Instance.getPool().storeLineGameObject(BaseModelBehavior.findModelBehavior<LineBehavior>(line));
            newLine = null;

            GameHelper.Instance.getLineDrawersManager().unregister(lineDrawer);
            lineDrawer = null;
        }

        if (newLine == null) {
            //do nothing more
            return;
        }

        line = newLine;

        GameObject goLine = GameHelper.Instance.getPool().pickLineGameObject(line);
        goLine.transform.SetParent(transform.parent);

        //put in background
        goLine.transform.SetAsFirstSibling();

        //retain to avoid deallocating during animation
        lineDrawer = new LineDrawer(line);
        GameHelper.Instance.getLineDrawersManager().register(lineDrawer);

        lineDrawer.show();

        updateLineColor();
    }

    void BaseNodeListener.onNodeReachableChange(BaseNode node) {

        updateLineColor();
    }


    protected void updateLineColor() {

        if (line == null) {
            return;
        }

        line.setColor(node.isReachable ? Constants.COLOR_LINE_DEFAULT : Constants.COLOR_LINE_INACTIVE);
    }
    

    public virtual void onNodeClick() {

        if (!isInit()) {
            return;
        }

        node.onNodeSelect();
    }

}
