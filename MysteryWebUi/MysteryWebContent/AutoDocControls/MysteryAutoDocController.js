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

// Defines the column user object
function Column(name) {
    this.name = name;
};

Column.prototype.type = 'string';

Column.prototype.primaryKey = false;

Column.prototype.clone = function () {
    return mxUtils.clone(this);
};

// Defines the table user object
function Table(name) {
    this.name = name;
};

Table.prototype.clone = function () {
    return mxUtils.clone(this);
};


app.directive('mxgraph', ["$timeout", function ($timeout) {
    var directive = {
        restrict: 'A',
        scope: { graph: "=graph", editor:"=editor"},
        link: function (scope, el, attrs) {
            // Program starts here. Gets the DOM elements for the respective IDs so things can be
            // created and wired-up.
            var graphContainer = el[0];

            if (!mxClient.isBrowserSupported()) {
                // Displays an error message if the browser is not supported.
                mxUtils.error('Browser is not supported!', 200, false);
            }
            else {
                // Creates the graph inside the given container. The
                // editor is used to create certain functionality for the
                // graph, such as the rubberband selection, but most parts
                // of the UI are custom in this example.
                var editor = new mxEditor();
                var graph = editor.graph;
                scope.editor = editor;
                scope.graph = graph;
                var model = graph.model;

                // Disables some global features
                graph.setConnectable(true);
                graph.setCellsDisconnectable(false);
                graph.setCellsCloneable(false);
                graph.swimlaneNesting = false;

                // Does not allow dangling edges
                graph.setAllowDanglingEdges(false);

                // Forces use of default edge in mxConnectionHandler
                graph.connectionHandler.factoryMethod = null;

                // Only tables are resizable
                graph.isCellResizable = function (cell) {
                    return this.isSwimlane(cell);
                };

                // Only tables are movable
                graph.isCellMovable = function (cell) {
                    return this.isSwimlane(cell);
                };

                configureStylesheet(graph);

                // Sets the graph container and configures the editor
                editor.setGraphContainer(graphContainer);

                // Configures the automatic layout for the table columns
                editor.layoutSwimlanes = true;
                editor.createSwimlaneLayout = function () {
                    var layout = new mxStackLayout(this.graph, false);
                    layout.fill = true;
                    layout.resizeParent = true;

                    // Overrides the function to always return true
                    layout.isVertexMovable = function (cell) {
                        return true;
                    };

                    return layout;
                };

                // Text label changes will go into the name field of the user object
                graph.model.valueForCellChanged = function (cell, value) {
                    if (value.name !== null) {
                        return mxGraphModel.prototype.valueForCellChanged.apply(this, arguments);
                    }
                    else {
                        var old = cell.value.name;
                        cell.value.name = value;
                        return old;
                    }
                };

                // Columns are dynamically created HTML labels
                graph.isHtmlLabel = function (cell) {
                    return !this.isSwimlane(cell) &&
                        !this.model.isEdge(cell);
                };

                // Returns the name field of the user object for the label
                graph.convertValueToString = function (cell) {
                    if (cell.value !== null && cell.value.name !== null) {
                        return cell.value.name;
                    }

                    return mxGraph.prototype.convertValueToString.apply(this, arguments); // "supercall"
                };

                // Removes the source vertex if edges are removed
                graph.addListener(mxEvent.REMOVE_CELLS, function (sender, evt) {
                    var cells = evt.getProperty('cells');

                    for (var i = 0; i < cells.length; i++) {
                        var cell = cells[i];

                        if (this.model.isEdge(cell)) {
                            var terminal = this.model.getTerminal(cell, true);
                            var parent = this.model.getParent(terminal);
                            this.model.remove(terminal);
                        }
                    }
                });

                // Adds child columns for new connections between tables
                graph.addEdge = function (edge, parent, source, target, index) {
                    // Finds the primary key child of the target table
                    var primaryKey = this.model.getChildAt(target, 0);
                    
                    this.model.beginUpdate();
                    try {
                        target = primaryKey;

                        return mxGraph.prototype.addEdge.apply(this, arguments); // "supercall"
                    }
                    finally {
                        this.model.endUpdate();
                    }
                };

            }
        }
    };

    return directive;
}]);

function htmlEntities(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
};

app.controller("MysteryAutoDocController",
    ['$scope', 'MysteryDownloader', '$translate', '$uibModal', '$location', '$sanitize',
        function ($scope, MysteryDownloader, $translate, $uibModal, $location, $sanitize) {
            var me = this;
            me.graph = null;
            me.editor = null;
            me.empty_xml = `<mxGraphModel>
  <root>
    <mxCell id="0"/>
    <mxCell id="1" parent="0"/>
  </root>
</mxGraphModel>`;
            me.xml = me.empty_xml;

            // the content type object
            var tableObject = new Table('TABLENAME');
            var table = new mxCell(tableObject, new mxGeometry(0, 0, 200, 28), 'table');
            table.setVertex(true);

            // Adds sidebar icon for the property object
            var columnObject = new Column('COLUMNNAME');
            var column = new mxCell(columnObject, new mxGeometry(0, 0, 0, 26));

            column.setVertex(true);
            column.setConnectable(false);

            // Adds primary key field into table
            var firstColumn = column.clone();

            firstColumn.value.name = 'Guid';
            firstColumn.value.type = 'Guid';
            firstColumn.value.primaryKey = true;

            table.insert(firstColumn);


            me.exportXml = function () {
                var enc = new mxCodec(mxUtils.createXmlDocument());
                var node = enc.encode(me.graph.getModel());
                var xml = mxUtils.getPrettyXml(node);
                $uibModal.open({
                    animation: true,
                    template: '<div >' + htmlEntities(xml)  + '</div>'
                });
                
            };

            MysteryDownloader.post('DownloadAutoDocMxGraphData', {}, function (result) {

                var parent = me.graph.getDefaultParent();
                var model = me.graph.getModel();
                var tables = {};

                //first the types
                angular.forEach(result.output, function (autodoc, index) {

                    var content_type_table = model.cloneCell(table);
                    tables[autodoc.name] = content_type_table;
                    model.beginUpdate();
                    try {
                        content_type_table.value.name = autodoc.name;
                        content_type_table.geometry.x = 0;
                        content_type_table.geometry.y = 0;

                        me.graph.addCell(content_type_table, parent);
                        content_type_table.geometry.alternateBounds = new mxRectangle(0, 0, content_type_table.geometry.width, content_type_table.geometry.height);

                    }
                    finally {
                        model.endUpdate();
                    }
                });

                //ready to connect!

                angular.forEach(result.output, function (autodoc, index) {
                    angular.forEach(autodoc.properties_names, function (property_name, index) {

                        var property_cell = model.cloneCell(column);

                        model.beginUpdate();
                        try {
                            property_cell.value.name = property_name;
                            property_cell.geometry.x = 0;
                            property_cell.geometry.y = 0;
                            me.graph.addCell(property_cell, tables[autodoc.name]);

                            if (angular.isDefined(autodoc.references[property_name])) {
                                me.graph.insertEdge(
                                    property_cell, null, property_name,
                                    property_cell,
                                    tables[autodoc.references[property_name].target_type]);
                            }

                        }
                        finally {
                            model.endUpdate();
                        }


                    });
                });


                // Creates a layout algorithm to be used
                // with the graph
                var layout = new mxFastOrganicLayout(me.graph);
                // Moves stuff wider apart than usual
                layout.forceConstant = 80;
                model.beginUpdate();
                try {
                    layout.execute(me.graph.getDefaultParent());
                }
                finally {
                    model.endUpdate();
                }
                
                

                
            });

            me.delete = function () {
                var cell = me.graph.getSelectionCell();
                me.editor.execute('delete', cell);
            };


            

        }]);