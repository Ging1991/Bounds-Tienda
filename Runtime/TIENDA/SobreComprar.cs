using System.Collections.Generic;
using Bounds.Cofres;
using Bounds.Modulos.Cartas.Ilustradores;
using Bounds.Modulos.Cartas.Tinteros;
using Bounds.Persistencia;
using Bounds.Persistencia.Datos;
using Ging1991.Core;
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
		public IlustradorDeCartas ilustrador;
		private ControlTiendaComprar controlTienda;

		void Start() {
			controlTienda = FindAnyObjectByType<ControlTiendaComprar>();
			coleccion = new Coleccion(codigo, controlTienda.carpetaColecciones.Generar(codigo));
			tintero = new TinteroBounds();
			ilustrador.Inicializar();

			GetComponent<ContenedorDeCartas>()?.Inicializar(ilustrador, tintero, coleccion.emblema.cartaID, coleccion.emblema.imagen);
			precioOBJ.GetComponent<MarcoConTexto>().SetTexto($"${precio}");
			precioOBJ.GetComponent<MarcoConTexto>().SetColorRelleno(Color.yellow);
			nombreOBJ.GetComponent<MarcoConTexto>().SetTexto($"{coleccion.titulo}");
			posesionOBJ.GetComponent<MarcoConTexto>().SetTexto($"{EstablecerPosesion()}");
		}


		protected string EstablecerPosesion() {
			Cofre cofre = new Cofre();
			List<CartaColeccionBD> cartas = coleccion.GetListaCompleta();
			List<int> cartasID = new List<int>();
			foreach (var carta in cartas) {
				if (!cartasID.Contains(carta.cartaID))
					cartasID.Add(carta.cartaID);
			}
			int cartasObtenidas = cofre.GetCantidadCartasDiferentes(cartasID);
			int cartasTotales = cartas.Count;
			int porcentaje = (int)(((float)cartasObtenidas / cartasTotales) * 100);
			return $"{cartasObtenidas}/{cartasTotales} ({porcentaje}%)";
		}


		void OnMouseDown() {
			Configuracion configuracion = controlTienda.configuracion;
			if (configuracion.LeerOro() >= precio)
				VentanaControl.CrearVentanaConfirmar($"¿Desea comprar el sobre por ${precio}?", this);
			else
				VentanaControl.CrearVentanaAceptar($"No tiene suficiente oro: ${configuracion.LeerOro()}");
		}


		public void PresionarBoton(TipoBoton boton) {
			if (boton == TipoBoton.ACEPTAR) {
				Configuracion configuracion = controlTienda.configuracion;
				configuracion.GastarOro(precio);

				GestorDeSobres lectorSobres = controlTienda.gestorDeSobres;
				lectorSobres.SetCantidad(coleccion.codigo, lectorSobres.GetCantidad(coleccion.codigo) + 1);
				GameObject.Find("Sonidos").GetComponent<GestorDeSonidos>().ReproducirSonido("FxAdquisicion");
			}
			if (boton == TipoBoton.CANCELAR) {
				GameObject.Find("Sonidos").GetComponent<GestorDeSonidos>().ReproducirSonido("FxRebote");
			}
		}


	}

}