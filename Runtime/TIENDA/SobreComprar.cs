using System.Collections.Generic;
using Bounds.Cartas;
using Bounds.Cofres;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using Bounds.Sistema;
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
		private IProveedor<string, string> traducciones;

		public void Inicializar(CartaGenerador cartaGenerador) {
			controlTienda = FindAnyObjectByType<ControlTiendaComprar>();
			coleccion = new Coleccion(codigo, controlTienda.carpetaColecciones.Generar(codigo));
			billetera = RegistroGlobal.Instancia.billetera;

			contenedorID.generador = cartaGenerador;
			contenedorID.MostrarCartaID(coleccion.emblema.cartaID, coleccion.emblema.imagen, "N");
			contenedorID.primitiva.SetTituloTexto($"{coleccion.titulo}", 0);
			contenedorID.primitiva.SetTituloTexto($"{EstablecerPosesion()}", 1);
			precioOBJ.SetTexto($"${precio}");
			traducciones = RegistroGlobal.Instancia.proveedorIdioma;
		}


		protected string EstablecerPosesion() {
			Cofre cofre = ControlTiendaComprar.Instancia.cofre;
			List<CartaColeccionBD> cartas = coleccion.GetListaCompleta();
			List<int> cartasID = new();
			foreach (var carta in cartas) {
				if (!cartasID.Contains(carta.cartaID))
					cartasID.Add(carta.cartaID);
			}
			int cartasObtenidas = cofre.GetCantidadCartasDiferentes(cartasID);
			int cartasTotales = cartasID.Count;
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
				ControlTiendaComprar.Instancia.ventanaControl.MostrarVentanaConfirmar(
					traducciones.GetElemento("DESEA_COMPRAR_POR_PRECIO").Replace("[PRECIO]", $"{precio}"), this);
			}
			else {
				ControlTiendaComprar.Instancia.gestorDeSonidos.ReproducirSonido("FxRebote");
				ControlTiendaComprar.Instancia.ventanaControl.MostrarVentanaAceptar(traducciones.GetElemento("NO_TIENE_ORO_PRECIO").Replace("[PRECIO]", $"{billetera.LeerOro()}"));
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