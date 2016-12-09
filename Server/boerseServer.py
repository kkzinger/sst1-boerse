from flask import Flask, request
import json
import datetime
app = Flask(__name__)

idBoerse = 'datBoerse'
@app.route('/')
def hello():
    return "<h1 style='color:blue'>Hello There!</h1>"

@app.route('/order', methods=['PUT','GET','DELETE'])
def order():

    #check if content-type is json, if not http status 400

        if request.method == 'PUT':

            if request.headers['Content-Type'] == 'application/json':
                data = request.get_json()

                try:
                    order = {}
                    order['id'] ='id4321'
                    order['idStock'] = data['idStock']
                    order['amount'] = data['amount']
                    order['price'] = data['price']
                    order['type'] = data['type']
                    order['timestamp'] = str(datetime.datetime.utcnow())
                    order['idBoerse'] = idBoerse
                    order['signature'] = 'sig-optional'
                    order['idBank'] = data['idBank']
                    order['idCustomer'] = data['idCustomer']
                    txlist = []
                    order['txhistory'] = txlist

                    return json.dumps(order), 200

                except Exception as error:
                    return 'Exception:\n{0}'.format(error), 500
            else:
                return 'Content of Request is not JSON',400

        elif request.method == 'GET':
            try:
                orderId = request.args.get('orderId')
                if orderId == None:
                    return 'No orderId provided',400

                elif orderId == 'id4321':
                    return '{"timestamp": "2012-12-15 11:15:24.984000", "amount": 7, "txhistory": [[5, 100.5], [2, 120.5]], "idBoerse": "datBoerse", "type": "buy", "idBank": "dieBank", "price": 100.5, "signature": "sig", "idStock": "st111", "idCustomer": "derCustomer", "id": "id4321"}',200

                elif orderId == 'id4344':
                    return '{"timestamp": "2012-12-15 12:15:24.984000", "amount": 15, "txhistory": [[5, 50.5], [2, 60.5]], "idBoerse": "datBoerse", "type": "buy", "idBank": "dieBank", "price": 60, "signature": "sig", "idStock": "st111", "idCustomer": "derCustomer1", "id": "id4344"}]',200

                elif orderId == '':
                    return '{[{"timestamp": "2012-12-15 11:15:24.984000", "amount": 7, "txhistory": [[5, 100.5], [2, 120.5]], "idBoerse": "datBoerse", "type": "buy", "idBank": "dieBank", "price": 100.5, "signature": "sig", "idStock": "st111", "idCustomer": "derCustomer", "id": "id4321"},{"timestamp": "2012-12-15 12:15:24.984000", "amount": 15, "txhistory": [[5, 50.5], [2, 60.5]], "idBoerse": "datBoerse", "type": "buy", "idBank": "dieBank", "price": 60, "signature": "sig", "idStock": "st111", "idCustomer": "derCustomer1", "id": "id4344"}]',200

                else:
                    return 'Dont have this orderId in System',400

            except Exception as error:
                return 'Exception:\n{0}'.format(error),500
        elif request.method == 'DELETE':
            return 'Not implemented',200
        else:
            return 'Reques Method is not allowed here',400

@app.route('/stock', methods=['GET'])
def stock():
    try:
        stock = {}
        stock1 = {}
        stock2 = {}
        stock['id'] = 'st8822'
        stock['name'] = 'Mighty Inc.'
        stock['price'] = 6900.44
        stock['idBoerse'] = idBoerse
        stock1['id'] = 'st7890'
        stock1['name'] = 'Poppel Inc.'
        stock1['price'] = 2.57
        stock1['idBoerse'] = idBoerse
        stock2['id'] = 'st666'
        stock2['name'] = 'Luzifer GmbH'
        stock2['price'] = 666.00
        stock2['idBoerse'] = idBoerse

        stocks = [stock,stock1,stock2]

        return json.dumps(stocks),200
    except Exception as error:
        return 'Exception:\n{0}'.format(error),500
#class order(object):
#  def __init__(self, id, idStock, amount, price, type, txhistory, timestamp, idBoerse, signature, idBank, idCustomer):
#     self.id = id
#     self.idStock = idStock
#     self.amount = amount
#     self.price = price
#     self.type = type
#     self.txhistory = txhistory
#     self.timestamp = timestamp
#     self.idBoerse = idBoerse
#     self.signature = signature
#     self.idBank = idBank
#     self.idCustomer = idCustomer
#
#class stock(object):
#    def __init__(self,id,name,price,idBoerse):
#        self.id = id
#        self.name = name
#        self.price = price
#        self.idBoerse = idBoerse
#
#def createOrder():
#    return 'foo'
if __name__ == "__main__":
    app.run(host='0.0.0.0')
