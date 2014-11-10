Array.prototype.pushUnique = function (item) {
    if (this.indexOf(item) == -1) this.push(item);
};

(function () {
    var app = angular.module('myAdmin', ['ngSanitize']);

    //Serviço para gerar uma instancia única do CodeMirror que será utilizada pelos controllers
    app.factory('CodeMirror', function () {

        if (document.getElementById('Query')) {
            var myCodeMirror = CodeMirror.fromTextArea(document.getElementById('Query'), {
                lineNumbers: true,
                extraKeys: { "Ctrl-Space": "autocomplete" },
                hint: CodeMirror.hint.sql,
                hintOptions: {
                    tables: {
                        'avaliacao': ['Id', 'Teste1', 'Teste2']
                    }
                },
                mode: { name: 'text/x-mssql' }
            });

            //$('.CodeMirror').resizable({
            //    resize: function () {
            //        editor.setSize($(this).width(), $(this).height());
            //    }
            //});

            return myCodeMirror;
        }
    });

    //https://github.com/maciel310/angular-filesystem
    app.factory('MyAdminFileSystem', ['$q', '$timeout', function ($q, $timeout) {
        var _this = this,
            fsDefer = $q.defer(),
            DEFAULT_QUOTA_MB = 1;

        window.resolveLocalFileSystemURL = window.resolveLocalFileSystemURL || window.webkitResolveLocalFileSystemURL;

        //wrap resolve/reject in an empty $timeout so it happens within the Angular call stack
        //easier than .apply() since no scope is needed and doesn't error if already within an apply
        function safeResolve(deferral, message) {
            $timeout(function () {
                deferral.resolve(message);
            });
        }
        function safeReject(deferral, message) {
            $timeout(function () {
                deferral.reject(message);
            });
        }

        //Inicialização
        if (angular.isDefined(navigator.webkitTemporaryStorage)) {
            navigator.webkitTemporaryStorage.requestQuota(DEFAULT_QUOTA_MB * 1024 * 1024, function (grantedBytes) {
                window.webkitRequestFileSystem(window.TEMPORARY, grantedBytes, function (fs) {
                    safeResolve(fsDefer, fs);
                }, function (e) {
                    safeReject(fsDefer, { text: "Error requesting File System access", obj: e });
                });
            }, function (e) {
                safeReject(fsDefer, { text: "Error requesting Quota", obj: e });
            });
        }

        var fileSystem = {
            writeConfigFile: function (file, mimeString) {
                var fileName = 'config.txt';
                var def = $q.defer();

                var reader = new FileReader();

                reader.onload = function (e) {
                    var buf = e.target.result;

                    $timeout(function () {
                        fileSystem.writeArrayBuffer(filename, buf, mimeString).then(function () {
                            safeResolve(def, "");
                        }, function (e) {
                            safeReject(def, e);
                        });
                    });
                };

                reader.readAsArrayBuffer(file);

                return def.promise;
            },
            writeConfigText: function (contents, append) {
                var fileName = 'config.txt';
                append = (typeof append == 'undefined' ? false : append);

                //create text blob from string
                var blob = new Blob([contents], { type: 'text/plain' });

                return fileSystem.writeBlob(fileName, blob, append);
            },
            writeArrayBuffer: function (fileName, buf, mimeString, append) {
                append = (typeof append == 'undefined' ? false : append);

                var blob = new Blob([new Uint8Array(buf)], { type: mimeString });

                return fileSystem.writeBlob(fileName, blob, append);
            },
            writeBlob: function (fileName, blob, append) {
                append = (typeof append == 'undefined' ? false : append);

                var def = $q.defer();

                fsDefer.promise.then(function (fs) {

                    fs.root.getFile(fileName, { create: true }, function (fileEntry) {

                        fileEntry.createWriter(function (fileWriter) {
                            if (append) {
                                fileWriter.seek(fileWriter.length);
                            }

                            var truncated = false;
                            fileWriter.onwriteend = function (e) {
                                //truncate all data after current position
                                if (!truncated) {
                                    truncated = true;
                                    this.truncate(this.position);
                                    return;
                                }
                                safeResolve(def, "");
                            };

                            fileWriter.onerror = function (e) {
                                safeReject(def, { text: 'Write failed', obj: e });
                            };

                            fileWriter.write(blob);

                        }, function (e) {
                            safeReject(def, { text: "Error creating file", obj: e });
                        });

                    }, function (e) {
                        safeReject(def, { text: "Error getting file", obj: e });
                    });

                }, function (err) {
                    def.reject(err);
                });

                return def.promise;
            },
            readConfigFile: function (returnType) {
                var fileName = 'config.txt';
                var def = $q.defer();

                returnType = returnType || "text";

                fsDefer.promise.then(function (fs) {
                    fs.root.getFile(fileName, {}, function (fileEntry) {
                        // Get a File object representing the file,
                        // then use FileReader to read its contents.
                        fileEntry.file(function (file) {
                            var reader = new FileReader();

                            reader.onloadend = function () {
                                safeResolve(def, this.result);
                            };

                            reader.onerror = function (e) {
                                safeReject(def, { text: "Error reading file", obj: e });
                            };


                            switch (returnType) {
                                case 'arraybuffer':
                                    reader.readAsArrayBuffer(file);
                                    break;
                                case 'binarystring':
                                    reader.readAsBinaryString(file);
                                    break;
                                case 'dataurl':
                                    reader.readAsDataURL(file);
                                    break;
                                default:
                                    reader.readAsText(file);
                            }
                        }, function (e) {
                            safeReject(def, { text: "Error getting file", obj: e });
                        });
                    }, function (e) {
                        safeReject(def, { text: "Error getting file", obj: e });
                    });
                }, function (err) {
                    def.reject(err);
                });

                return def.promise;
            }
        };

        // Keep old name for backwards compatibility
        fileSystem.requestQuotaIncrease = fileSystem.requestQuota;

        return fileSystem;
    }]);

    app.controller('QueryController', ['$http', '$scope', '$compile', 'CodeMirror', 'MyAdminFileSystem', function ($http, $scope, $compile, CodeMirror, myFs) {
        var _this = this;

        _this.sistema = 'Sistema';
        _this.ambiente = 'Ambiente';
        _this.banco = 'SqlServer';
        _this.query = '';
        _this.NomeArquivo = '';
        _this.resultado = '';
        _this.executing = false;
        _this.infoDBVisible = false;
        _this.limitedFooter = true;
        _this.dbStructure = [];
        _this.dados = '';
        _this.connections = [];
        _this.sistemas = [];
        _this.ambientes = [];

        _this.setSistema = function (value) {
            var list = _this.connections.filter(function (item) { return item.sistema == value });

            if (list.length == 0) {
                _this.sistema = 'Sistema';
                localStorage.removeItem('sistema');

                _this.ambiente = 'Ambiente';
                localStorage.removeItem('ambiente');
            }
            else {
                localStorage['sistema'] = value;
                _this.sistema = value;

                _this.ambientes = [];

                for (var i = 0; i < list.length; i++) {
                    _this.ambientes.pushUnique(list[i].ambiente.label);
                }

                _this.GetStructureDB();
            }
        }

        _this.setAmbiente = function (value) {
            localStorage['ambiente'] = value;
            _this.ambiente = value;
            _this.GetStructureDB();
        }

        _this.setBanco = function (value) {
            localStorage['banco'] = value;
            _this.banco = value;
        }

        _this.setLastQuery = function (value) {
            if (value)
                localStorage['lastQuery'] = value;
        }

        _this.getLastQuery = function () {
            if (localStorage['lastQuery'])
                CodeMirror.setValue(localStorage['lastQuery']);
        }

        _this.getActiveText = function (prefix) {
            var selection = CodeMirror.getSelection();
            var token = CodeMirror.getTokenAt(CodeMirror.getCursor());
            if (token) token = token.string.trim();


            return prefix ? prefix + (selection ? selection : token) : (selection ? selection : CodeMirror.getValue());
        }

        _this.getConnectionString = function () {
            var conn = _this.connections.filter(function (item) { return item.sistema == _this.sistema && item.ambiente.label == _this.ambiente });
            if (conn.length == 1) {
                return conn[0].connString;
            } else {
                _this.resultado = 'Sistema/Ambiente não cadastrado nas configurações';
            }
            return null;
        }

        _this.saveQueryFile = function () {
            var query = CodeMirror.getValue();
            if (query) {
                var blob = new Blob([query], { type: "text/plain;charset=utf-8" });
                saveAs(blob, "query.txt");
            }
        }

        _this.toogleFooter = function () {
            _this.limitedFooter = !_this.limitedFooter;
        }

        _this.buildHtmlTable = function (data) {
            var htmlBody = '<tbody>', htmlHead = '';

            $(data).each(function (i, itemTr) {
                //Para montar o cabeçalho da tabela
                if (i == 0) {
                    htmlHead += '<thead>';
                    for (var itemTd in itemTr) {
                        htmlHead += '<td><strong>' + itemTd + '</strong></td>';
                    }
                    htmlHead += '</thead>';
                }

                htmlBody += '<tr>';

                for (var itemTd in itemTr) {
                    var value = $(itemTr).prop(itemTd);

                    if ((typeof value == 'string') && value.indexOf('/Date(') == 0) {
                        var re = /-?\d+/;

                        var m = re.exec(value);

                        value = new Date(parseInt(m[0])).toLocaleDateString();
                    }

                    htmlBody += '<td>' + value + '</td>';
                }

                htmlBody += '</tr>';
            });
            htmlBody += '</tbody>'

            return $('<table class="table table-striped"></table>').append(htmlHead + htmlBody)
        }

        _this.executaQuery = function (salvaArquivo, prefix) {
            salvaArquivo = salvaArquivo ? salvaArquivo : false;

            //Salva última query executada
            _this.setLastQuery(CodeMirror.getValue());

            var connectionString = _this.getConnectionString();

            if (connectionString) {
                _this.query = _this.getActiveText(prefix);

                $form = $('#form-query');

                _this.resultado = '';
                _this.executing = true;

                var data = JSON.stringify({
                    Sistema: _this.sistema,
                    Ambiente: _this.ambiente,
                    Banco: _this.banco,
                    Query: _this.query,
                    ConnectionString: connectionString,
                    SalvaArquivo: salvaArquivo,
                    NomeArquivo: _this.NomeArquivo
                });

                $http.post($form.attr('action'), data)
                    .success(function (data, status, headers, config) {
                        if (data.msg)
                            _this.resultado = data.msg;

                        _this.dados = '';
                        $(data.dataTables).each(function (i, item) {
                            if (data.fileSaved) {
                                //console.log(item);
                                _this.resultado = _this.resultado.replace(item, '<a href="' + item + '">' + item + '</a>');
                            } else {
                                _this.dados += _this.buildHtmlTable(item.dataTable)[0].outerHTML;
                            }
                            _this.resultado += '(' + item.rowsAffected + ' row(s) affected) <br/>';
                        });

                        //TODO: Acertar isso utilizando directives
                        setInterval(function () { $('body').attr('style', 'padding-bottom:' + $('.app-footer').height() + 'px'); }, 1000);

                    }).error(function (data, status, headers, config) {
                        _this.resultado = data.msg ? data.msg : "Problema na execução da consulta";

                        console.log(data);
                        console.log(status);
                        console.log(headers);
                        console.log(config);
                    }).finally(function () {
                        _this.executing = false;
                    });
            }
        }

        _this.salvaArquivo = function () {
            _this.executaQuery(true);
        }

        _this.toggleInfoDB = function () {
            _this.infoDBVisible = !_this.infoDBVisible;
        }

        _this.GetStructureDB = function () {
            if (_this.sistema == 'Sistema')
                return;
            if (_this.ambiente == 'Ambiente')
                return;

            function getColumnsArray(columns) {
                var listColumns = [];
                for (var i = 0; i < columns.length; i++){
                    listColumns.push(columns[i].Nome);
                }
                return listColumns;
            }

            _this.resultado = '';
            _this.executing = true;

            var data = JSON.stringify({
                Sistema: _this.sistema,
                Ambiente: _this.ambiente,
                Banco: _this.banco,
                ConnectionString: _this.getConnectionString()
            });
            $http.post('Home/GetStructureDB', data)
              .success(function (data, status, headers, config) {
                  if (data.msg)
                      _this.resultado = data.msg;

                  _this.dbStructure = data.data;

                  //Ajusta a estrutura de acordo com o pluguin sql-hint do CodeMirror e atualiza o hintOptions.
                  var cmHintOptions = { tables: {} };
                  for (var i = 0; i < data.data.Tabelas.length; i++) {
                      cmHintOptions.tables[data.data.Tabelas[i].Nome] = getColumnsArray(data.data.Tabelas[i].Colunas);
                  }
                  CodeMirror.options.hintOptions = cmHintOptions;

              }).error(function (data, status, headers, config) {
                  _this.resultado = data.msg ? data.msg : "Problema buscando metadados do banco";

                  console.log(data);
                  console.log(status);
                  console.log(headers);
                  console.log(config);
              }).finally(function () {
                  _this.executing = false;
              });

        }

        //Execuções iniciais
        myFs.readConfigFile().then(function (msg) {
            _this.connections = JSON.parse(msg);

            for (var i = 0; i < _this.connections.length; i++) {
                _this.sistemas.pushUnique(_this.connections[i].sistema);
            }

            if (localStorage['sistema']) {
                _this.setSistema(localStorage['sistema']);
            }
            if (localStorage['ambiente']) {
                _this.setAmbiente(localStorage['ambiente']);
            }
            if (localStorage['banco']) {
                _this.setBanco(localStorage['banco']);
            }
        });

        //Configurando Atalhos
        shortcut.add("F5", function () {
            _this.executaQuery();

            //TODO: Achar melhor forma de fazer
            $compile($('footer'))($scope)

            return false;
        });

        shortcut.add("Ctrl+1", function () {
            var prefix = localStorage['ctrl1'] ? localStorage['ctrl1'] + ' ' : 'select * from ';
            _this.executaQuery(false, prefix);

            //TODO: Achar melhor forma de fazer
            $compile($('footer'))($scope)
            return false;
        });

        shortcut.add("Ctrl+2", function () {
            var prefix = localStorage['ctrl2'] ? localStorage['ctrl2'] + ' ' : 'select top 10 * from ';
            _this.executaQuery(false, prefix);

            //TODO: Achar melhor forma de fazer
            $compile($('footer'))($scope)
            return false;
        });

        shortcut.add("Ctrl+3", function () {
            var prefix = localStorage['ctrl3'] ? localStorage['ctrl3'] + ' ' : 'sp_help ';
            _this.executaQuery(false, prefix);

            //TODO: Achar melhor forma de fazer
            $compile($('footer'))($scope)
            return false;
        });
    }]);

    app.controller('EditorController', ['CodeMirror', function (CodeMirror) {
        this.theme = 'default';

        this.setTheme = function (theme) {
            localStorage['theme'] = theme;
            this.theme = theme;
            $('.CodeMirror').attr('class', 'CodeMirror cm-s-' + theme);
        }

        this.themeClick = function (theme) {
            this.setTheme(theme);
            $('.query-theme-icon').popover('hide');
        }

        //Execuções iniciais
        if (localStorage['theme']) {
            this.setTheme(localStorage['theme']);
        }
    }]);

    //Como o popover é montado dinamicamente ele tem que ser recompilado pelo Angular, para os binds funcionarem corretamente com o escopo
    app.directive('popoverThemes', function ($compile) {
        return {
            restrict: 'E',
            template: $('#popover-tmpl-options').html(),
            link: function (scope, elements, attrs) {

                $('.query-theme-icon').popover({
                    title: 'Temas',
                    html: true,
                    trigger: 'click focus',
                    placement: 'left',
                    content: function () {
                        return $compile($('#popover-tmpl-options').html())(scope);
                    },
                });
            }
        }
    });

    app.controller('ConfigurationController', ['$timeout', 'MyAdminFileSystem', function (timeout, myFs) {
        var _this = this;
        _this.ctrl1 = localStorage['ctrl1'] ? localStorage['ctrl1'] : 'select * from';
        _this.ctrl2 = localStorage['ctrl2'] ? localStorage['ctrl2'] : 'select top 10 * from';
        _this.ctrl3 = localStorage['ctrl3'] ? localStorage['ctrl3'] : 'sp_help';
        _this.connections = [];
        _this.ambientes = [{ label: 'Desenvolvimento', value: 'des' }, { label: 'Homologação', value: 'hom' }, { label: 'Produção', value: 'pro' }];
        _this.saveVisible = false;
        _this.saveJsonVisible = false;
        _this.panelJsonVisible = false;
        _this.connection = {};
        _this.jsonConnections = '';

        //Execuções iniciais
        myFs.readConfigFile().then(function (msg) {
            _this.connections = JSON.parse(msg);
        });

        _this.save = function () {
            localStorage['ctrl1'] = _this.ctrl1;
            localStorage['ctrl2'] = _this.ctrl2;
            localStorage['ctrl3'] = _this.ctrl3;

            _this.saveVisible = true;
            timeout(function () { _this.saveVisible = false; }, 5000);
        }

        _this.addConnectionString = function () {
            if (!$.isEmptyObject(_this.connection)
                && _this.connection.sistema
                && _this.connection.connString
                && !$.isEmptyObject(_this.connection.ambiente)) {

                _this.connections.push(_this.connection);
                myFs.writeConfigText(JSON.stringify(_this.connections));
                _this.connection = {};
            }
        }

        _this.deleteConnectionString = function (conn) {
            _this.connections = _this.connections.filter(function (obj) { return !(obj.sistema == conn.sistema && obj.ambiente.value == conn.ambiente.value && obj.connString == conn.connString) });
            myFs.writeConfigText(JSON.stringify(_this.connections));
        }

        _this.getJsonConnections = function () {
            alert(JSON.stringify(_this.connections));
        }

        _this.setJsonConnections = function () {
            if (_this.jsonConnections.trim()) {
                _this.connections = JSON.parse(_this.jsonConnections);
                myFs.writeConfigText(JSON.stringify(_this.connections));

                _this.saveJsonVisible = true;
                timeout(function () { _this.saveJsonVisible = false; _this.togleJsonPanel(); }, 5000);

                _this.jsonConnections = '';
            }
        }

        _this.togleJsonPanel = function () {
            _this.panelJsonVisible = !_this.panelJsonVisible;
        }
    }]);

})();