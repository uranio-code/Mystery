function configureStylesheet(graph) {
    var style = new Object();
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_RECTANGLE;
    style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_LEFT;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_MIDDLE;
    style[mxConstants.STYLE_FONTCOLOR] = '#000000';
    style[mxConstants.STYLE_FONTSIZE] = '11';
    style[mxConstants.STYLE_FONTSTYLE] = 0;
    style[mxConstants.STYLE_SPACING_LEFT] = '4';
    style[mxConstants.STYLE_IMAGE_WIDTH] = '48';
    style[mxConstants.STYLE_IMAGE_HEIGHT] = '48';
    graph.getStylesheet().putDefaultVertexStyle(style);

    style = new Object();
    style[mxConstants.STYLE_SHAPE] = mxConstants.SHAPE_SWIMLANE;
    style[mxConstants.STYLE_PERIMETER] = mxPerimeter.RectanglePerimeter;
    style[mxConstants.STYLE_ALIGN] = mxConstants.ALIGN_CENTER;
    style[mxConstants.STYLE_VERTICAL_ALIGN] = mxConstants.ALIGN_TOP;
    style[mxConstants.STYLE_GRADIENTCOLOR] = '#41B9F5';
    style[mxConstants.STYLE_FILLCOLOR] = '#8CCDF5';
    style[mxConstants.STYLE_SWIMLANE_FILLCOLOR] = '#ffffff';
    style[mxConstants.STYLE_STROKECOLOR] = '#1B78C8';
    style[mxConstants.STYLE_FONTCOLOR] = '#000000';
    style[mxConstants.STYLE_STROKEWIDTH] = '2';
    style[mxConstants.STYLE_STARTSIZE] = '28';
    style[mxConstants.STYLE_VERTICAL_ALIGN] = 'middle';
    style[mxConstants.STYLE_FONTSIZE] = '12';
    style[mxConstants.STYLE_FONTSTYLE] = 1;
    style[mxConstants.STYLE_IMAGE] = 'images/icons48/table.png';
    // Looks better without opacity if shadow is enabled
    //style[mxConstants.STYLE_OPACITY] = '80';
    style[mxConstants.STYLE_SHADOW] = 1;
    graph.getStylesheet().putCellStyle('table', style);

    style = graph.stylesheet.getDefaultEdgeStyle();
    style[mxConstants.STYLE_LABEL_BACKGROUNDCOLOR] = '#FFFFFF';
    style[mxConstants.STYLE_STROKEWIDTH] = '2';
    style[mxConstants.STYLE_ROUNDED] = true;
    style[mxConstants.STYLE_EDGE] = mxEdgeStyle.EntityRelation;
};


app.directive('mxgraph', ["$timeout", function ($timeout) {
    var directive = {
        restrict: 'A',
        link: function (scope, el, attrs) {
            var app = Sys.Application;
            app.add_init(function (sender, args) {
                // Program starts here. Gets the DOM elements for the respective IDs so things can be
                // created and wired-up.
                var graphContainer = el[0];//$get('graphContainer

                if (!mxClient.isBrowserSupported()) {
                    // Displays an error message if the browser is not supported.
                    mxUtils.error('Browser is not supported!', 200, false);
                }
                else {
                    // Creates an instance of the graph control, passing the graphContainer element to the
                    // mxGraph constructor. The $create function is part of ASP.NET. It can take an ID for
                    // creating objects so the new instances can later be found using the $find function.
                    var graphControl = $create(aspnet.GraphControl, null, null, null, graphContainer);

                    scope.$watch(attrs.mxgraph, function (value) {
                        var doc = mxUtils.parseXml(value);

                        // Adds cells to the model in a single step
                        graphControl.get_graph().getModel().beginUpdate();
                        try {
                            graphControl.decode(doc.documentElement);
                        }
                        finally {
                            // Updates the display
                            graphControl.get_graph().getModel().endUpdate();
                        }
                    });

                    configureStylesheet(graphControl.get_graph());

                    
                }
            });
        }
    };

    return directive;
}]);


app.controller("MysteryAutoDocController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location',
        function ($scope, MysteryDownloader, $translate, $uibModal, $location) {
            var me = this;
            me.base_xml = `<mxGraphModel>
  <root>
    <mxCell id="0"/>
    <mxCell id="1" parent="0"/>
    <mxCell id="2" style="table" vertex="1" parent="1">
      <Table name="TABLE1" as="value"/>
      <mxGeometry x="90" y="190" width="200" height="80" as="geometry">
        <mxRectangle width="200" height="28" as="alternateBounds"/>
      </mxGeometry>
    </mxCell>
    <mxCell id="3" vertex="1" connectable="0" parent="2">
      <Column name="TABLE1_ID" type="INTEGER" primaryKey="1" autoIncrement="1" as="value"/>
      <mxGeometry y="28" width="200" height="26" as="geometry"/>
    </mxCell>
    <mxCell id="6" vertex="1" connectable="0" parent="2">
      <Column name="TABLE2_ID" type="INTEGER" as="value"/>
      <mxGeometry y="54" width="200" height="26" as="geometry"/>
    </mxCell>
    <mxCell id="4" style="table" vertex="1" parent="1">
      <Table name="TABLE2" as="value"/>
      <mxGeometry x="240" y="420" width="200" height="54" as="geometry">
        <mxRectangle width="200" height="28" as="alternateBounds"/>
      </mxGeometry>
    </mxCell>
    <mxCell id="5" vertex="1" connectable="0" parent="4">
      <Column name="TABLE2_ID" type="INTEGER" primaryKey="1" autoIncrement="1" as="value"/>
      <mxGeometry y="28" width="200" height="26" as="geometry"/>
    </mxCell>
    <mxCell id="7" edge="1" parent="1" source="6" target="5">
      <mxGeometry relative="1" as="geometry"/>
    </mxCell>
  </root>
</mxGraphModel>`;
            me.xml = me.base_xml;

            me.wipe = function () {
                me.xml = `<mxGraphModel>
  <root>
    <mxCell id="0"/>
    <mxCell id="1" parent="0"/>
  </root>
</mxGraphModel>`;
            };
            me.load = function () {
                me.xml = me.base_xml;
            };

        }]);