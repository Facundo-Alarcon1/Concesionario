using System;
using System.Collections.Generic;
using System.Windows.Forms;
using concesionario;
using Controller;
using Model.Clases;
using Microsoft.VisualBasic;
using System.Net.NetworkInformation; // Necesario para Interaction.InputBox

namespace WindowsFormsApp
{ 
    public partial class GerenteControl : UserControl
    {
        private AutoController autoController;
        private List<Auto> autosDisponibles;
        public int idEmpleadoActual;
        public string puestoActual;

        // Campos para el menú contextual
        private ContextMenuStrip contextMenuAutos = new ContextMenuStrip();
        private Auto autoSeleccionadoContextMenu;
        private Empleados empleadoActualContextMenu;

        public GerenteControl(int idEmpleado, string puesto)
        {
            InitializeComponent();
            autoController = new AutoController();
            idEmpleadoActual = idEmpleado;
            puestoActual = puesto;

            CargarAutosDisponibles();
            ConfigurarDataGridView();

            // Oculta el botón 'Agregar' si no es gerente
            if (puestoActual.ToLower() != "gerente")
            {
                btnAgregar.Visible = false;
            }
        }

        private void GerenteControl_Load(object sender, EventArgs e)
        {
            // No es necesario recargar aquí
        }

        private void CargarAutosDisponibles()
        {
            autosDisponibles = autoController.BuscarAutosPorEstado("disponible");
            dataGridViewAutos.DataSource = null;
            dataGridViewAutos.DataSource = autosDisponibles;
        }

        private void ConfigurarDataGridView()
        {
            dataGridViewAutos.CellClick -= DataGridViewAutos_CellClick;
            dataGridViewAutos.CellClick += DataGridViewAutos_CellClick;

            // Elimina columnas de botones existentes
            List<string> columnsToRemove = new List<string>();
            foreach (DataGridViewColumn col in dataGridViewAutos.Columns)
            {
                if (col is DataGridViewButtonColumn)
                    columnsToRemove.Add(col.Name);
            }
            foreach (string colName in columnsToRemove)
                dataGridViewAutos.Columns.Remove(colName);

            // Solo botón "Vender"
            var btnVender = new DataGridViewButtonColumn
            {
                Name = "btnVender",
                HeaderText = "Vender",
                Text = "Vender",
                UseColumnTextForButtonValue = true
            };
            dataGridViewAutos.Columns.Add(btnVender);

            // Suscribir evento para clic derecho
            dataGridViewAutos.CellMouseDown -= DataGridViewAutos_CellMouseDown;
            dataGridViewAutos.CellMouseDown += DataGridViewAutos_CellMouseDown;
        }

        private void DataGridViewAutos_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dataGridViewAutos.ClearSelection();
                dataGridViewAutos.Rows[e.RowIndex].Selected = true;
                autoSeleccionadoContextMenu = autosDisponibles[e.RowIndex];
                empleadoActualContextMenu = new Empleados { ID_empleado = idEmpleadoActual, Puesto = puestoActual };

                // Reconstruir el menú contextual con los handlers correctos
                contextMenuAutos.Items.Clear();
                contextMenuAutos.Items.Add("Modificar Marca", null, (s, ev) => ModificarMarcaAuto(autoSeleccionadoContextMenu, empleadoActualContextMenu));
                contextMenuAutos.Items.Add("Modificar Modelo", null, (s, ev) => ModificarModeloAuto(autoSeleccionadoContextMenu, empleadoActualContextMenu));
                contextMenuAutos.Items.Add("Modificar Color", null, (s, ev) => ModificarColorAuto(autoSeleccionadoContextMenu, empleadoActualContextMenu));
                contextMenuAutos.Items.Add("Modificar Patente", null, (s, ev) => ModificarPatenteAuto(autoSeleccionadoContextMenu, empleadoActualContextMenu));
                contextMenuAutos.Items.Add("Modificar Año", null, (s, ev) => ModificarAnioAuto(autoSeleccionadoContextMenu, empleadoActualContextMenu));
                contextMenuAutos.Items.Add("Modificar Precio", null, (s, ev) => ModificarPrecioAuto(autoSeleccionadoContextMenu));
                contextMenuAutos.Items.Add("Eliminar", null, (s, ev) => EliminarAuto(autoSeleccionadoContextMenu, empleadoActualContextMenu));

                contextMenuAutos.Show(Cursor.Position);
            }
        }


       

        private void DataGridViewAutos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= autosDisponibles.Count)
                return;

            if (dataGridViewAutos.Columns[e.ColumnIndex].Name == "btnVender")
            {
                var autoSeleccionado = autosDisponibles[e.RowIndex];
                VenderAuto(autoSeleccionado);
                CargarAutosDisponibles();
            }
        }

        // --- Métodos de Acción ---

        public void btnAgregarAuto_Click(object sender, EventArgs e)
        {
            string marca = Interaction.InputBox("Ingrese la marca del auto:", "Agregar Auto", "");
            if (string.IsNullOrEmpty(marca)) return;

            string modelo = Interaction.InputBox("Ingrese el modelo del auto:", "Agregar Auto", "");
            if (string.IsNullOrEmpty(modelo)) return;

            string color = Interaction.InputBox("Ingrese el color del auto:", "Agregar Auto", "");
            if (string.IsNullOrEmpty(color)) return;

            string patente = Interaction.InputBox("Ingrese la patente del auto:", "Agregar Auto", "");
            if (string.IsNullOrEmpty(patente)) return;

            string anioStr = Interaction.InputBox("Ingrese el año del auto:", "Agregar Auto", "");
            if (string.IsNullOrEmpty(anioStr)) return;

            string precioStr = Interaction.InputBox("Ingrese el precio del auto:", "Agregar Auto", "");
            if (string.IsNullOrEmpty(precioStr)) return;

            if (!int.TryParse(anioStr, out int anio) || !decimal.TryParse(precioStr, out decimal precio))
            {
                MessageBox.Show("Datos inválidos. Por favor, complete todos los campos correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var nuevoAuto = new Auto
            {
                Marca = marca,
                Modelo = modelo,
                Color = color,
                Patente = patente,
                Anio = anio,
                Estado = "disponible",
                ID_empleado = idEmpleadoActual,
                Precio = precio
            };

            if (autoController.AgregarAuto(nuevoAuto, new Empleados { ID_empleado = idEmpleadoActual, Puesto = puestoActual }))
            {
                MessageBox.Show("El auto fue agregado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarAutosDisponibles();
            }
            else
            {
                MessageBox.Show("Error al agregar el auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            string criterio = txtBuscar.Text.Trim();

            if (!string.IsNullOrEmpty(criterio))
            {
                autosDisponibles = autoController.BuscarAuto(criterio);
            }
            else
            {
                autosDisponibles = autoController.BuscarAutosPorEstado("disponible");
            }

            dataGridViewAutos.DataSource = null;
            dataGridViewAutos.DataSource = autosDisponibles;
        }

        private void VenderAuto(Auto autoSeleccionado)
        {
            var confirmResult = MessageBox.Show($"¿Está seguro de que desea vender el auto {autoSeleccionado.Marca} {autoSeleccionado.Modelo}?",
                                                 "Confirmar Venta",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                if (autoController.VenderAuto(autoSeleccionado.ID_auto, idEmpleadoActual, "Venta realizada"))
                {
                    MessageBox.Show($"El auto {autoSeleccionado.Marca} {autoSeleccionado.Modelo} fue vendido correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAutosDisponibles();
                }
                else
                {
                    MessageBox.Show("Error al vender el auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ModificarPrecioAuto(Auto autoSeleccionado)
        {
            string nuevoPrecioStr = Interaction.InputBox("Ingrese el nuevo precio del auto:", "Modificar Precio", autoSeleccionado.Precio.ToString());
            if (decimal.TryParse(nuevoPrecioStr, out decimal nuevoPrecio))
            {
                if (autoController.ActualizarPrecioAuto(autoSeleccionado.ID_auto, nuevoPrecio))
                {
                    MessageBox.Show($"El precio del auto {autoSeleccionado.Marca} {autoSeleccionado.Modelo} fue actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAutosDisponibles();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el precio del auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (!string.IsNullOrEmpty(nuevoPrecioStr))
            {
                MessageBox.Show("El precio ingresado no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ModificarMarcaAuto(Auto autoSeleccionado, Empleados empleadoActual)
        {
            string nuevaMarca = Interaction.InputBox("Ingrese la nueva marca:", "Modificar Marca", autoSeleccionado.Marca);
            if (!string.IsNullOrEmpty(nuevaMarca))
            {
                if (autoController.ActualizarMarcaAuto(autoSeleccionado.ID_auto, nuevaMarca))
                {
                    MessageBox.Show($"La marca del auto fue actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAutosDisponibles();
                }
                else
                {
                    MessageBox.Show("Error al actualizar la marca del auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ModificarModeloAuto(Auto autoSeleccionado, Empleados empleadoActual)
        {
            string nuevoModelo = Interaction.InputBox("Ingrese el nuevo modelo:", "Modificar Modelo", autoSeleccionado.Modelo);
            if (!string.IsNullOrEmpty(nuevoModelo))
            {
                if (autoController.ActualizarModeloAuto(autoSeleccionado.ID_auto, nuevoModelo))
                {
                    MessageBox.Show($"El modelo del auto fue actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAutosDisponibles();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el modelo del auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ModificarColorAuto(Auto autoSeleccionado, Empleados empleadoActual)
        {
            string nuevoColor = Interaction.InputBox("Ingrese el nuevo color:", "Modificar Color", autoSeleccionado.Color);
            if (!string.IsNullOrEmpty(nuevoColor))
            {
                if (autoController.ActualizarColorAuto(autoSeleccionado.ID_auto, nuevoColor))
                {
                    MessageBox.Show($"El color del auto fue actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAutosDisponibles();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el color del auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ModificarPatenteAuto(Auto autoSeleccionado, Empleados empleadoActual)
        {
            string nuevaPatente = Interaction.InputBox("Ingrese la nueva patente:", "Modificar Patente", autoSeleccionado.Patente);
            if (!string.IsNullOrEmpty(nuevaPatente))
            {
                if (autoController.ActualizarPatenteAuto(autoSeleccionado.ID_auto, nuevaPatente))
                {
                    MessageBox.Show($"La patente del auto fue actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAutosDisponibles();
                }
                else
                {
                    MessageBox.Show("Error al actualizar la patente del auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ModificarAnioAuto(Auto autoSeleccionado, Empleados empleadoActual)
        {
            string nuevoAnioStr = Interaction.InputBox("Ingrese el nuevo año:", "Modificar Año", autoSeleccionado.Anio.ToString());
            if (int.TryParse(nuevoAnioStr, out int nuevoAnio))
            {
                if (autoController.ActualizarAnioAuto(autoSeleccionado.ID_auto, nuevoAnio))
                {
                    MessageBox.Show($"El año del auto fue actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAutosDisponibles();
                }
                else
                {
                    MessageBox.Show("Error al actualizar el año del auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    
                }
            }
            else if (!string.IsNullOrEmpty(nuevoAnioStr))
            {
                MessageBox.Show("El año ingresado no es válido.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EliminarAuto(Auto autoSeleccionado, Empleados empleadoActual)
        {
            var confirmResult = MessageBox.Show($"¿Está seguro de que desea eliminar el auto {autoSeleccionado.Marca} {autoSeleccionado.Modelo} (ID: {autoSeleccionado.ID_auto})? Esta acción es irreversible.",
                                                 "Confirmar Eliminación",
                                                 MessageBoxButtons.YesNo,
                                                 MessageBoxIcon.Warning);

            if (confirmResult == DialogResult.Yes)
            {
                if (autoController.EliminarAuto(autoSeleccionado.ID_auto))
                {
                    MessageBox.Show($"El auto {autoSeleccionado.Marca} {autoSeleccionado.Modelo} fue eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    CargarAutosDisponibles();
                }
                else
                {
                    MessageBox.Show("Error al eliminar el auto. Intente nuevamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // --- Botones para navegación ---

        private void btnComprobantes_Click(object sender, EventArgs e)
        {
            this.Hide();
            ComprobantesControl comprobantesControl = new ComprobantesControl(idEmpleadoActual, puestoActual);
            comprobantesControl.Dock = DockStyle.Fill;
            this.Parent.Controls.Add(comprobantesControl);
            comprobantesControl.BringToFront();
        }

        private void BtnIrAServicio_Click(object sender, EventArgs e)
        {
            this.Hide();
            ServicioControl servicioControl = new ServicioControl(idEmpleadoActual, puestoActual);
            servicioControl.Dock = DockStyle.Fill;
            this.Parent.Controls.Add(servicioControl);
            servicioControl.BringToFront();
        }
    }
}
