using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FashionStore.Models
{
    public class Cart : cart
    {
        dbDoryShopDataContext db = new dbDoryShopDataContext();
        public DateTime date_added { get; set; }
        public decimal price { get; set; }
        public decimal discount_price { get; set; }
        public decimal amount { get { return quantity * discount_price; } }

        //Khởi tạo giỏ hàng với số lượng mặc định 1;
        public Cart(string product_code)
        {
            var product = db.GetProductDetail(product_code).FirstOrDefault();
            this.name = product.name;
            this.product_code = product_code;
            this.product_id = product.product_id;
            this.price = product.price;
            this.discount_price = product.discount_price;
            this.image = product.image;
            this.quantity = 1;
            this.date_added = DateTime.Now;
        }

    }
}