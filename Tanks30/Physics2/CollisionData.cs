
namespace Physics
{
    /// <summary>
    /// Datos de colisi�n
    /// </summary>
    public class CollisionData
    {
        /// <summary>
        /// N�mero m�ximo de contactos
        /// </summary>
        protected const int _MaxContacts = 256;

        /// <summary>
        /// Indice del contacto actual
        /// </summary>
        private int m_CurrentContactIndex = 0;
        /// <summary>
        /// Lista de contactos
        /// </summary>
        private Contact[] m_ContactArray;

        /// <summary> 
        /// Factor de fricci�n a a�adir en todas las colisiones
        /// </summary>
        public float Friction = 0f;
        /// <summary> 
        /// Factor de restituci�n a a�adir en todas las colisiones
        /// </summary>
        public float Restitution = 0f;
        /// <summary>
        /// Tolerancia
        /// </summary>
        public float Tolerance = 0f;

        /// <summary>
        /// Lista de contactos
        /// </summary>
        public Contact[] ContactArray
        {
            get
            {
                return m_ContactArray;
            }
        }
        /// <summary> 
        /// Contacto actual
        /// </summary>
        public Contact CurrentContact
        {
            get
            {
                return m_ContactArray[m_CurrentContactIndex];
            }
        }
        /// <summary> 
        /// N�mero de contactos usados
        /// </summary>
        public int ContactCount
        {
            get
            {
                return m_CurrentContactIndex;
            }
        }
        /// <summary> 
        /// N�mero de contactos libres
        /// </summary>
        public int ContactsLeft
        {
            get
            {
                return m_ContactArray.Length - (m_CurrentContactIndex);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CollisionData()
            : this(_MaxContacts)
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maxContacts">N�mero de contactos de la lista de contactos</param>
        public CollisionData(int maxContacts)
        {
            this.InitializeContactArray(maxContacts);
        }

        /// <summary>
        /// Obtiene si hay m�s contactos disponibles en la lista de contactos
        /// </summary>
        public bool HasFreeContacts()
        {
            return m_CurrentContactIndex < m_ContactArray.Length;
        }
        /// <summary>
        /// Restablece la lista de contactos
        /// </summary>
        public void Reset()
        {
            m_CurrentContactIndex = 0;
        }
        /// <summary>
        /// Restablece la lista de contactos al tama�o especificado
        /// </summary>
        /// <param name="maxContacts">N�mero de contactos de la lista de contactos</param>
        public void Reset(int maxContacts)
        {
            if (m_ContactArray.Length != maxContacts)
            {
                this.InitializeContactArray(maxContacts);
            }

            this.Reset();
        }
        /// <summary>
        /// Notifica a la instancia que se ha a�adido un contacto.
        /// </summary>
        public void AddContact()
        {
            this.m_CurrentContactIndex++;
        }

        /// <summary>
        /// Inicializa la lista de contactos al n�mero especificado
        /// </summary>
        /// <param name="maxContacts">N�mero de contactos de la lista de contactos</param>
        private void InitializeContactArray(int maxContacts)
        {
            m_ContactArray = new Contact[maxContacts];
            for (int i = 0; i < m_ContactArray.Length; i++)
            {
                m_ContactArray[i] = new Contact();
            }
        }
    }
}