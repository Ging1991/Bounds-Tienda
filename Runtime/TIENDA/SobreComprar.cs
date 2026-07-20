using System.Collections.Generic;
using Bounds.Cartas;
using Bounds.Cofres;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using Ging1991.Core.Interfaces;
using Ging1991.Interfaces.Salida;
using UnityEngine;

namespace Bounds.Tienda {

	public class SobreComprar : MonoBehaviour, IEjecutable {

		private Coleccion coleccion;
		public string codigo;
		public int precio;
		private ControlTiendaComprar controlTienda;
		public ContenedorID contenedorID;
		public MarcoConTexto precioOBJ;
		private Billetera billetera;

		public void Inicializar(CartaGenerador cartaGenerador) {
			controlTienda = FindAnyObjectByType<ControlTiendaComprar>();
			coleccion = new Coleccion(codigo, controlTienda.carpetaColecciones.Generar(codigo));
			billetera = RegistroGlobal.Instancia.billetera;

			contenedorID.generador = cartaGenerador;
			contenedorID.MostrarCartaID(coleccion.emblema.cartaID, coleccion.emblema.imagen, "N");
			contenedorID.primitiva.SetTituloTexto($"{coleccion.titulo}", 0);
			contenedorID.primitiva.SetTituloTexto($"{EstablecerPosesion()}", 1);
			precioOBJ.SetTexto($"${precio}");
		}


		protected string EstablecerPosesion() {
			Cofre cofre = ControlTiendaComprar.Instancia.cofre;
			List<CartaColeccionBD> cartasColeccion = coleccion.GetListaCompleta();
			List<string> cartasID = new();
			foreach (var cartaColeccion in cartasColeccion) {
				cartasID.Add(GetCodigoFormato(cartaColeccion.cartaID, cartaColeccion.imagen));
			}
			int cartasObtenidas = cofre.GetCantidadCartasPorColeccion(cartasID);
			int cartasTotales = cartasColeccion.Count;
			int porcentaje = (int)(((float)cartasObtenidas / cartasTotales) * 100);
			return $"{cartasObtenidas}/{cartasTotales} ({porcentaje}%)";
		}


		protected string GetCodigoFormato(int cartaID, string imagen) {
			if (cartaID < 10)
				return $"00{cartaID}_{imagen}";
			if (cartaID < 100)
				return $"0{cartaID}_{imagen}";
			return $"{cartaID}_{imagen}";
		}


		void OnMouseUpAsButton() {
			if (billetera.LeerOro() >= precio) {
				ControlTiendaComprar.Instancia.ventanaControl.MostrarVentanaConfirmar($"¿Desea comprar el sobre por ${precio}?", this);
			}
			else {
				ControlTiendaComprar.Instancia.gestorDeSonidos.ReproducirSonido("FxRebote");
				ControlTiendaComprar.Instancia.ventanaControl.MostrarVentanaAceptar($"No tiene suficiente oro: ${billetera.LeerOro()}");
			}
		}


		public void Ejecutar() {
			billetera.GastarOro(precio);
			GestorDeSobres lectorSobres = controlTienda.gestorDeSobres;
			lectorSobres.SetCantidad(coleccion.codigo, lectorSobres.GetCantidad(coleccion.codigo) + 1);
			ControlTiendaComprar.Instancia.gestorDeSonidos.ReproducirSonido("FxAdquisicion");
		}
	}

}