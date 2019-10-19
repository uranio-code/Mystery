app.controller("AutoDocGraphController",
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

            me.content = null;
            me.setGrapth = function (content) {
                me.content = content;
                MysteryDownloader.loadContent(content, function (content) {
                    var shapes = [];
                    var map = {};

                    angular.forEach(content.shapes, function (value) {
                        shapes.push(value.auto_doc_type);
                    });
                    MysteryDownloader.loadContents(shapes, function () {
                        var parent = me.graph.getDefaultParent();
                        var model = me.graph.getModel();
                        var tables = {};
                        var references = [];


                        me.graph.addListener(mxEvent.MOVE_CELLS, function (sender, evt) {
                            var cell = evt.properties.cells[0];
                            if (cell !== null) {
                                var content_type_name = cell.value.name;
                                if (angular.isDefined(map[content_type_name])) {
                                    map[content_type_name].x = cell.geometry.x;
                                    map[content_type_name].y = cell.geometry.y;
                                    MysteryDownloader.post("SaveGraphShape", { grap_guid: me.content.guid, shape: map[content_type_name] });
                                }
                            }
                        });

                        //first the types
                        angular.forEach(content.shapes, function (shape) {
                            var autodoc = shape.auto_doc_type;
                            map[autodoc.name] = shape;

                            var content_type_table = model.cloneCell(table);
                            angular.forEach(autodoc.properties_names, function (property_name) {
                                // Adds field into table
                                var a_property = column.clone();

                                a_property.value.name = property_name;
                                content_type_table.insert(a_property);
                                if (angular.isDefined(autodoc.references[property_name])) {
                                    references.push({ source: a_property, target: autodoc.references[property_name].target_type });
                                }
                            });

                            tables[autodoc.name] = content_type_table;
                            model.beginUpdate();
                            try {
                                content_type_table.value.name = autodoc.name;
                                content_type_table.geometry.x = shape.x;
                                content_type_table.geometry.y = shape.y;

                                me.graph.addCell(content_type_table, parent);
                                content_type_table.geometry.alternateBounds = new mxRectangle(0, 0, content_type_table.geometry.width, content_type_table.geometry.height);

                            }
                            finally {
                                model.endUpdate();
                            }
                        });

                        //ready to connect!
                        angular.forEach(references, function (reference) {
                            if (angular.isDefined(tables[reference.target])) {
                                model.beginUpdate();
                                try {
                                    me.graph.insertEdge(
                                        reference.source, null, '',
                                        reference.source,
                                        tables[reference.target]);
                                }
                                finally {
                                    model.endUpdate();
                                }
                            }
                        });
                    });
                });
            };
            

           

            me.delete = function () {
                var cell = me.graph.getSelectionCell();
                me.editor.execute('delete', cell);
            };

            

            


            

        }]);