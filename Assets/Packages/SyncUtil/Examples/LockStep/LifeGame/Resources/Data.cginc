struct Data{
	int alive;
};

int _Width;
int _Height;

inline int xyToIdx(int2 xy)
{
    return xy.y * _Width  + xy.x;
}

