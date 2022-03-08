using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LittleSheep
{
    public struct RemoteUser
    {
        public string userName;
        public IPEndPoint endPoint;

        public String UserName { get; set; }
        public String EndPoint
        {
            get
            {
                return endPoint.ToString();
            }
            set
            {
                string[] endPontStr = value.Split(':');
                endPoint = new IPEndPoint(IPAddress.Parse(endPontStr[0]), Convert.ToInt32(endPontStr[1]));
            }
        }

        public RemoteUser(string userName, IPEndPoint endPoint)
        {
            this.userName = userName;
            this.endPoint = endPoint;
            UserName = userName;
        }

        #region 重载的函数
        public override string ToString()
        {
            return userName + " " + endPoint.ToString();
        }

        public bool Equals(RemoteUser other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // 由于发送消息用的是随机端口，所以这里只判断IP是否相同
            if (userName != other.userName || endPoint.Address.ToString() != other.endPoint.Address.ToString()) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RemoteUser)obj);
        }

        public override int GetHashCode()
        {
            return ((endPoint != null && userName != null) ? endPoint.Address.ToString().GetHashCode() + userName.GetHashCode() : 0);
        }

        public static bool operator ==(RemoteUser left, RemoteUser right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RemoteUser left, RemoteUser right)
        {
            return !Equals(left, right);
        }
        #endregion

    }
}
