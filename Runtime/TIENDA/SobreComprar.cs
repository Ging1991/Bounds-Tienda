using System.Collections.Generic;
using Bounds.Cofres;
using Bounds.Modulos.Cartas.Tinteros;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using Ging1991.Core.Interfaces;
using Ging1991.Interfaces;
using Ging1991.Ventanas;
using UnityEngine;

namespace Bounds.Tienda {

	public class SobreComprar : MonoBehaviour, IPresionarBoton {

		private Coleccion coleccion;
		public string codigo;
		public int precio;
		public GameObject precioOBJ;
		public GameObject nombreOBJ;
		public GameObject posesionOBJ;
		private ITintero tintero;
		public IProveedor<string, Sprite> ilustrador;
		private ControlTiendaComprar controlTienda;

		public void Inicializar() {
			controlTienda = FindAnyObjectByType<ControlTiendaComprar>();
			coleccion = new Coleccion(codigo, controlTienda.carpetaColecciones.Generar(codigo));
			tintero = new TinteroBounds();
			ilustrador = FindAnyObjectByType<ControlTiendaComprar>().selectorImagenes;

			GetComponent<ContenedorDeCartas>()?.Inicializar(ilustrador, tintero, coleccion.emblema.cartaID, coleccion.emblema.imagen);
			precioOBJ.GetComponent<MarcoConTexto>().SetTexto($"${precio}");
			precioOBJ.GetComponent<MarcoConTexto>().SetColorRelleno(Color.yellow);
			nombreOBJ.GetComponent<MarcoConTexto>().SetTexto($"{coleccion.titulo}");
			posesionOBJ.GetComponent<MarcoConTexto>().SetTexto($"{EstablecerPosesion()}");
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


		void OnMouseDown() {
			Billetera billetera = controlTienda.billetera;
			if (billetera.LeerOro() >= precio)
				VentanaControl.CrearVentanaConfirmar($"¿Desea comprar el sobre por ${precio}?", this);
			else {
				ControlTiendaComprar.Instancia.gestorDeSonidos.ReproducirSonido("FxRebote");
				VentanaControl.CrearVentanaAceptar($"No tiene suficiente oro: ${billetera.LeerOro()}");
			}
		}


		public void PresionarBoton(TipoBoton boton) {
			if (boton == TipoBoton.ACEPTAR) {
				Billetera billetera = controlTienda.billetera;
				billetera.GastarOro(precio);

				GestorDeSobres lectorSobres = controlTienda.gestorDeSobres;
				lectorSobres.SetCantidad(coleccion.codigo, lectorSobres.GetCantidad(coleccion.codigo) + 1);
				ControlTiendaComprar.Instancia.gestorDeSonidos.ReproducirSonido("FxAdquisicion");
			}
			if (boton == TipoBoton.CANCELAR) {
				ControlTiendaComprar.Instancia.gestorDeSonidos.ReproducirSonido("FxRebote");
			}
		}


	}

}